import { RoleType, ObjectType } from '../meta';
import { ParametrizedPredicate } from './ParametrizedPredicate';
import { UnitTypes } from '../workspace';
import { serializeArray } from '../workspace/SessionObject';

export class Between extends ParametrizedPredicate {
  dependencies: string[];
  roleType: RoleType;
  parameter: string;
  values: UnitTypes[];

  constructor(fields?: Partial<Between> | RoleType) {
    super();

    if ((fields as RoleType).objectType) {
      this.roleType = fields as RoleType;
    } else {
      Object.assign(this, fields);
    }
  }

  get objectType(): ObjectType {
    return this.roleType.objectType;
  }

  toJSON(): any {
    return {
      kind: 'Between',
      dependencies: this.dependencies,
      roleType: this.roleType.id,
      parameter: this.parameter,
      values: serializeArray(this.values),
    };
  }
}
