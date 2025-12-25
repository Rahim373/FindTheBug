import { Directive, Input, TemplateRef, ViewContainerRef, OnInit } from '@angular/core';
import { PermissionService, PermissionType } from '../services/permission.service';

@Directive({
    selector: '[hasPermission]',
    standalone: true
})
export class HasPermissionDirective implements OnInit {
    @Input() hasPermission!: string; // Module name
    @Input() hasPermissionType: PermissionType = PermissionType.View;
    @Input() hasPermissionElse?: TemplateRef<any>;

    constructor(
        private templateRef: TemplateRef<any>,
        private viewContainer: ViewContainerRef,
        private permissionService: PermissionService
    ) {}

    ngOnInit(): void {
        const hasAccess = this.permissionService.hasPermissionSync(this.hasPermission, this.hasPermissionType);
        
        if (hasAccess) {
            this.viewContainer.clear();
            this.viewContainer.createEmbeddedView(this.templateRef);
        } else if (this.hasPermissionElse) {
            this.viewContainer.clear();
            this.viewContainer.createEmbeddedView(this.hasPermissionElse);
        } else {
            this.viewContainer.clear();
        }
    }
}