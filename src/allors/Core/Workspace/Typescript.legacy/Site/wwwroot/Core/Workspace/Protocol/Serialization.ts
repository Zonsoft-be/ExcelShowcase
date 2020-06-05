namespace Allors.Protocol {
  import ids = Meta.ids;
  import ObjectType = Meta.ObjectType;

  export type UnitTypes = string | Date | boolean | number;
  export type CompositeTypes = ISessionObject | string;

  export function serializeObject(roles: { [name: string]: UnitTypes; }): { [name: string]: string; } {
    if (roles) {
      return Object
        .keys(roles)
        .reduce((obj, v) => {
          obj[v] = serialize(roles[v]);
          return obj;
        }, {});
    }

    return {};
  }

  export function serializeArray(roles: UnitTypes[]): string[] {
    if (roles) {
      return roles.map(v => serialize(v));
    }

    return [];
  }

  export function serialize(role: UnitTypes): string {

    if (role === undefined || role === null) {
      return null;
    }

    if (typeof role === 'string') {
      return role;
    }

    if (role instanceof Date) {
      return (role as Date).toISOString();
    }

    return role.toString();
  }

  export function deserialize(value: string, objectType: ObjectType): UnitTypes {
    switch (objectType.id) {
    case ids.Boolean:
      return value === 'true' ? true : false;
    case ids.Float:
      return parseFloat(value);
    case ids.Integer:
      return parseInt(value, 10);
    }

    return value;
  }

}
