import { domain } from '../domain';
import { Person } from '../generated/Person.g';
import { Meta } from '../../meta';

declare module '../generated/Person.g' {
    interface Person {
        displayName: string;

        hello(): string;
    }
}

domain.extend((workspace) => {

    const m = workspace.metaPopulation as Meta;
    const person = workspace.constructorByObjectType.get(m.Person)?.prototype as any;

    person.hello = function(this: Person) {
        return `Hello ${this.displayName}`;
    };

    Object.defineProperty(person, 'displayName', {
        get(this: Person): string {
            if (this.FirstName || this.LastName) {
                if (this.FirstName && this.LastName) {
                    return this.FirstName + ' ' + this.LastName;
                } else if (this.LastName) {
                    return this.LastName;
                } else if (this.FirstName){
                    return this.FirstName;
                }
            }

            if (this.UserName) {
                return this.UserName;
            }

            return 'N/A';
        },
    });
});
