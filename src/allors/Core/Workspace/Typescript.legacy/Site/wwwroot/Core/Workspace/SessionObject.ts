/// <reference path="./Protocol/Push/PushRequest.ts" />
namespace Allors {
    import ObjectType = Meta.ObjectType;
    import RoleType = Meta.RoleType;
    import AssociationType = Meta.AssociationType;
    import MethodType = Meta.MethodType;
    import OperandType = Meta.OperandType;

    import Operations = Protocol.Operations;
    import Compressor = Protocol.Compressor;
    import PushRequestObject = Protocol.PushRequestObject;
    import PushRequestNewObject = Protocol.PushRequestNewObject;
    import PushRequestRole = Protocol.PushRequestRole;
    import serialize = Protocol.serialize;

    export interface IObject {
        id: string;
        objectType: ObjectType;
    }

    export interface ISessionObject extends IObject {
        id: string;
        newId: string;
        version: string;
        objectType: ObjectType;

        isNew: boolean;

        session: ISession;
        workspaceObject: IWorkspaceObject;

        hasChanges: boolean;

        canRead(roleType: RoleType): boolean;
        canWrite(roleTyp: RoleType): boolean;
        canExecute(methodType: MethodType): boolean;
        isPermited(operandType: OperandType, operation: Operations): boolean;

        get(roleType: RoleType): any;
        set(roleType: RoleType, value: any);
        add(roleType: RoleType, value: any);
        remove(roleType: RoleType, value: any);

        getAssociation(associationType: AssociationType): any;

        save(compressor: Compressor): PushRequestObject;
        saveNew(compressor: Compressor): PushRequestNewObject;
        reset();
    }

    export class SessionObject implements ISessionObject {

        public session: Session;
        public workspaceObject: IWorkspaceObject;
        public objectType: ObjectType;
        public newId: string;
        private changedRoleByRoleType: Map<RoleType, any>;
        private roleByRoleType: Map<RoleType, any>;

        get isNew(): boolean {
            return this.newId ? true : false;
        }

        get hasChanges(): boolean {
            if (this.newId) {
                return true;
            }

            return !!this.changedRoleByRoleType;
        }

        get id(): string {
            return this.workspaceObject ? this.workspaceObject.id : this.newId;
        }

        get version(): string {
            return this.workspaceObject ? this.workspaceObject.version : undefined;
        }

        public canRead(roleType: RoleType | string): boolean {
            if (typeof roleType === 'string') {
                // TODO:
                return true;
            }

            return this.isPermited(roleType, Operations.Read);
        }

        public canWrite(roleType: RoleType | string): boolean {
            if (typeof roleType === 'string') {
                // TODO:
                return true;
            }

            return this.isPermited(roleType, Operations.Write);
        }

        public canExecute(methodType: MethodType | string): boolean {
            if (typeof methodType === 'string') {
                // TODO:
                return true;
            }

            return this.isPermited(methodType, Operations.Execute);
        }

        public isPermited(operandType: OperandType, operation: Operations): boolean {
            if (this.roleByRoleType === undefined) {
                return undefined;
            }

            if (this.newId) {
                return true;
            } else if (this.workspaceObject) {
                const permission = this.session.workspace.permission(this.objectType, operandType, operation);
                return this.workspaceObject.isPermitted(permission);
            }

            return false;
        }

        public method(methodType: MethodType): Method {
            if (this.roleByRoleType === undefined) {
                return undefined;
            }

            return new Method(this, methodType);
        }

        public get(roleType: RoleType): any {
            if (this.roleByRoleType === undefined) {
                return undefined;
            }

            let value = this.roleByRoleType.get(roleType);
            if (value === undefined) {
                if (this.newId === undefined) {
                    if (roleType.objectType.isUnit) {
                        value = this.workspaceObject.roleByRoleTypeId.get(roleType.id);
                        if (value === undefined) {
                            value = null;
                        }
                    } else {
                        try {
                            if (roleType.isOne) {
                                const role: string = this.workspaceObject.roleByRoleTypeId.get(roleType.id);
                                value = role ? this.session.get(role) : null;
                            } else {
                                const roles: string[] = this.workspaceObject.roleByRoleTypeId.get(roleType.id);
                                value = roles ? roles.map((role) => {
                                    return this.session.get(role);
                                }) : [];
                            }
                        } catch (e) {
                            let stringValue = 'N/A';
                            try {
                                stringValue = this.toString();
                            } catch (e2) {
                                throw new Error(`Could not get role ${roleType.name} from [objectType: ${this.objectType.name}, id: ${this.id}]`);
                            }

                            throw new Error(`Could not get role ${roleType.name} from [objectType: ${this.objectType.name}, id: ${this.id}, value: '${stringValue}']`);
                        }
                    }
                } else {
                    if (roleType.objectType.isComposite && roleType.isMany) {
                        value = [];
                    } else {
                        value = null;
                    }
                }

                this.roleByRoleType.set(roleType, value);
            }

            return value;
        }

        public getForAssociation(roleType: RoleType): any {
            if (this.roleByRoleType === undefined) {
                return undefined;
            }

            let value = this.roleByRoleType.get(roleType);
            if (value === undefined) {
                if (this.newId === undefined) {
                    if (roleType.objectType.isUnit) {
                        value = this.workspaceObject.roleByRoleTypeId.get(roleType.id);
                        if (value === undefined) {
                            value = null;
                        }
                    } else {
                        if (roleType.isOne) {
                            const role: string = this.workspaceObject.roleByRoleTypeId.get(roleType.id);
                            value = role ? this.session.getForAssociation(role) : null;
                        } else {
                            const roles: string[] = this.workspaceObject.roleByRoleTypeId.get(roleType.id);
                            value = roles ? roles.map((role) => {
                                return this.session.getForAssociation(role);
                            }) : [];
                        }
                    }
                } else {
                    if (roleType.objectType.isComposite && roleType.isMany) {
                        value = [];
                    } else {
                        value = null;
                    }
                }

                this.roleByRoleType.set(roleType, value);
            }

            return value;
        }

        public set(roleType: RoleType, value: any) {
            this.assertExists();

            if (this.changedRoleByRoleType === undefined) {
                this.changedRoleByRoleType = new Map();
            }

            if (value === undefined) {
                value = null;
            }

            if (value === null) {
                if (roleType.objectType.isComposite && roleType.isMany) {
                    value = [];
                }
            }

            if (value === '') {
                if (roleType.objectType.isUnit) {
                    if (!roleType.objectType.isString) {
                        value = null;
                    }
                }
            }

            this.roleByRoleType.set(roleType, value);
            this.changedRoleByRoleType.set(roleType, value);

            this.session.hasChanges = true;
        }

        public add(roleType: RoleType, value: any) {
            if (!!value) {
                this.assertExists();

                const roles = this.get(roleType);
                if (roles.indexOf(value) < 0) {
                    roles.push(value);
                }

                this.set(roleType, roles);

                this.session.hasChanges = true;
            }
        }

        public remove(roleType: RoleType, value: any) {
            if (!!value) {
                this.assertExists();

                const roles = this.get(roleType);
                const index = roles.indexOf(value);
                if (index >= 0) {
                    roles.splice(index, 1);
                }

                this.set(roleType, roles);

                this.session.hasChanges = true;
            }
        }

        public getAssociation(associationType: AssociationType): any {
            this.assertExists();

            const associations = this.session.getAssociation(this, associationType);

            if (associationType.isOne) {
                return associations.length > 0 ? associations[0] : null;
            }

            return associations;
        }

        public save(compressor: Compressor): PushRequestObject {
            if (this.changedRoleByRoleType !== undefined) {
                const data = new PushRequestObject();
                data.i = this.id;
                data.v = this.version;
                data.roles = this.saveRoles(compressor);
                return data;
            }

            return undefined;
        }

        public saveNew(compressor: Compressor): PushRequestNewObject {
            this.assertExists();

            const data = new PushRequestNewObject();
            data.ni = this.newId;
            data.t = compressor.write(this.objectType.id);

            if (this.changedRoleByRoleType !== undefined) {
                data.roles = this.saveRoles(compressor);
            }

            return data;
        }

        public reset() {
            if (this.newId) {
                delete this.newId;
                delete this.session;
                delete this.objectType;
                delete this.roleByRoleType;
            } else {
                this.workspaceObject = this.workspaceObject.workspace.get(this.id);
                this.roleByRoleType = new Map();
            }

            delete this.changedRoleByRoleType;
        }

        public onDelete(deleted: SessionObject) {
            if (this.changedRoleByRoleType !== undefined) {

                for (const [roleType, value] of this.changedRoleByRoleType) {
                    if (!roleType.objectType.isUnit) {
                        if (roleType.isOne) {
                            const role = value as SessionObject;
                            if (role && role === deleted) {
                                this.set(roleType, null);
                            }
                        } else {
                            const roles = value as SessionObject[];
                            if (roles && roles.indexOf(deleted) > -1) {
                                this.remove(roleType, deleted);
                            }
                        }
                    }
                }
            }
        }

        protected init() {
            this.roleByRoleType = new Map();
        }

        private assertExists() {
            if (this.roleByRoleType === undefined) {
                throw new Error('Object doesn\'t exist anymore.');
            }
        }

        private saveRoles(compressor: Compressor): PushRequestRole[] {
            const saveRoles = new Array<PushRequestRole>();

            if (this.changedRoleByRoleType) {

                for (const [roleType, value] of this.changedRoleByRoleType) {
                    const saveRole = new PushRequestRole();
                    saveRole.t = compressor.write(roleType.id);

                    let role = value;
                    if (roleType.objectType.isUnit) {
                        role = serialize(role);
                        saveRole.s = role;
                    } else {
                        if (roleType.isOne) {
                            saveRole.s = role ? role.id || role.newId : null;
                        } else {
                            const roleIds = role.map((item) => (item as SessionObject).id || (item as SessionObject).newId);
                            if (this.newId) {
                                saveRole.a = roleIds;
                            } else {
                                const originalRoleIds = this.workspaceObject.roleByRoleTypeId.get(roleType.id) as string[];
                                if (!originalRoleIds) {
                                    saveRole.a = roleIds;
                                } else {
                                    saveRole.a = roleIds.filter((v) => originalRoleIds.indexOf(v) < 0);
                                    saveRole.r = originalRoleIds.filter((v) => roleIds.indexOf(v) < 0);
                                }
                            }
                        }
                    }

                    saveRoles.push(saveRole);
                }

                return saveRoles;
            }
        }
    }

}
