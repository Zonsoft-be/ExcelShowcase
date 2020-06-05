import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, Self } from '@angular/core';

import {  ContextService, MetaService } from '../../../../../angular';
import { ContactMechanismPurpose, Country, PartyContactMechanism, PostalAddress } from '../../../../../domain';
import { PullRequest, Sort, Equals } from '../../../../../framework';
import { Meta } from '../../../../../meta';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'party-contactmechanism-postaladdress',
  templateUrl: './postaladdress-inline.component.html',
})
export class PartyContactMechanismPostalAddressInlineComponent implements OnInit, OnDestroy {

  @Output()
  public saved: EventEmitter<PartyContactMechanism> = new EventEmitter<PartyContactMechanism>();

  @Output()
  public cancelled: EventEmitter<any> = new EventEmitter();

  public countries: Country[];
  public contactMechanismPurposes: ContactMechanismPurpose[];

  public partyContactMechanism: PartyContactMechanism;
  public postalAddress: PostalAddress;

  public m: Meta;

  constructor(
    private allors: ContextService,
    public metaService: MetaService) {

    this.m = this.metaService.m;
  }

  public ngOnInit(): void {

    const { m, pull, x } = this.metaService;

    const pulls = [
      pull.Country({
        sort: new Sort(this.m.Country.Name)
      }),
      pull.ContactMechanismPurpose({
        predicate: new Equals({ propertyType: this.m.ContactMechanismPurpose.IsActive, value: true }),
        sort: new Sort(this.m.ContactMechanismPurpose.Name)
      })
    ];

    this.allors.context
      .load(new PullRequest({ pulls }))
      .subscribe((loaded) => {
        this.countries = loaded.collections.Countries as Country[];
        this.contactMechanismPurposes = loaded.collections.ContactMechanismPurposes as ContactMechanismPurpose[];

        this.partyContactMechanism = this.allors.context.create('PartyContactMechanism') as PartyContactMechanism;
        this.postalAddress = this.allors.context.create('PostalAddress') as PostalAddress;
        this.partyContactMechanism.ContactMechanism = this.postalAddress;
      });
  }

  public ngOnDestroy(): void {

    if (!!this.partyContactMechanism) {
      this.allors.context.delete(this.partyContactMechanism);
      this.allors.context.delete(this.postalAddress);
    }
  }

  public cancel(): void {
    this.cancelled.emit();
  }

  public save(): void {
    this.saved.emit(this.partyContactMechanism);

    this.partyContactMechanism = undefined;
    this.postalAddress = undefined;
  }
}
