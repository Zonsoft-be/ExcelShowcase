import { Fetch } from './Fetch';

export class Result {
  public fetchRef: string;

  public fetch: Fetch;

  public name: string;

  public skip: number;

  public take: number;

  constructor(fields?: Partial<Result>) {
    Object.assign(this, fields);
  }
}
