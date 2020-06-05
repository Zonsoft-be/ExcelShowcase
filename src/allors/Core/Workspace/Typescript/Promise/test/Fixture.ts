import { domain } from '../src/allors/domain';
import { MetaPopulation, Workspace } from '../src/allors/framework';
import { data, Meta, TreeFactory, FetchFactory, PullFactory } from '../src/allors/meta';
import { Database, Context } from '../src/allors/promise';

import { AxiosHttp } from '../src/allors/promise/core/http/AxiosHttp';
import { runInThisContext } from 'vm';

export class Fixture {

  readonly FULL_POPULATION = 'full';

  metaPopulation: MetaPopulation;
  m: Meta;
  ctx: Context;

  pull: PullFactory;
  tree: TreeFactory;
  fetch: FetchFactory;

  readonly x = new Object();

  private http: AxiosHttp;

  async init(population?: string) {

    this.http = new AxiosHttp({ baseURL: 'http://localhost:5000/' });

    this.metaPopulation = new MetaPopulation(data);
    this.m = this.metaPopulation as Meta;
    const workspace = new Workspace(this.metaPopulation);
    domain.apply(workspace);

    await this.login('administrator');
    await this.http.get('Test/Setup', { population });
    const database = new Database(this.http);
    this.ctx = new Context(database, workspace);

    this.tree = new TreeFactory(this.m);
    this.fetch = new FetchFactory(this.m);
    this.pull = new PullFactory(this.m);

  }

  async login(user: string) {
    await this.http.login('TestAuthentication/Token', user);
  }
}
