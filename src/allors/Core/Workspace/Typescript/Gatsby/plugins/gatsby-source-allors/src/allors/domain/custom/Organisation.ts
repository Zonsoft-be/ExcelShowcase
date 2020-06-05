import { domain } from '../domain';
import { Meta } from '../../meta';
import { Organisation } from '../generated';

import { createSlug } from '../../gatsby/utils/createSlug';

declare module '../generated/Organisation.g' {
  interface Organisation {
    slug;
  }
}

export const Slug = 'slug';

domain.extend((workspace) => {

  const m = workspace.metaPopulation as Meta;
  const organisation = workspace.constructorByObjectType.get(m.Organisation).prototype as any;

  Object.defineProperty(organisation, Slug, {
    enumerable: true,
    get(this: Organisation): string {
      return createSlug(this.Name);
    },
  });
});
