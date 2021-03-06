import { Injectable } from '@angular/core';

import { Pull } from '../../../framework';
import { Loaded } from '../framework';

import { PanelManagerService } from './panelmanager.service';

@Injectable({
    providedIn: 'root',
})
export class PanelService {

    name: string;
    title: string;
    icon: string;
    expandable: boolean;

    onPull: (pulls: Pull[]) => void;
    onPulled: (loaded: Loaded) => void;

    constructor(public manager: PanelManagerService) {
        manager.panels.push(this);
    }

    get isCollapsed(): boolean {
        return !this.manager.expanded;
    }

    get isExpanded(): boolean {
        return this.manager.expanded === this.name;
    }

    toggle() {
        this.manager.toggle(this.name);
    }

}
