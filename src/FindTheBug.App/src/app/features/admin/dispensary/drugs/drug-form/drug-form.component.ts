import { Component, OnInit, inject } from '@angular/core';

import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { DrugService } from '../../../../../core/services/drug.service';
import { DrugType, Brand, GenericName } from '../../../../../core/models/drug.models';

@Component({
    selector: 'app-drug-form',
    standalone: true,
    imports: [
    ReactiveFormsModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzSelectModule,
    NzInputNumberModule,
    NzSwitchModule
],
    templateUrl: './drug-form.component.html',
    styleUrls: ['./drug-form.component.css']
})
export class DrugFormComponent implements OnInit {
    private readonly fb = inject(FormBuilder);
    private readonly drugService = inject(DrugService);
    private readonly router = inject(Router);
    private readonly route = inject(ActivatedRoute);
    private readonly message = inject(NzMessageService);

    drugForm!: FormGroup;
    isEditMode = false;
    drugId: string | null = null;
    loading = false;
    brands: Brand[] = [];
    genericNames: GenericName[] = [];
    drugTypes = Object.values(DrugType);

    ngOnInit(): void {
        this.initForm();
        this.loadBrands();
        this.loadGenericNames();

        this.drugId = this.route.snapshot.paramMap.get('id');
        if (this.drugId) {
            this.isEditMode = true;
            this.loadDrug(this.drugId);
        }
    }

    initForm(): void {
        this.drugForm = this.fb.group({
            name: ['', [Validators.required]],
            strength: ['', [Validators.required]],
            genericNameId: ['', [Validators.required]],
            brandId: ['', [Validators.required]],
            type: ['', [Validators.required]],
            unitPrice: [0, [Validators.required, Validators.min(0)]],
            photoPath: [''],
            isActive: [true]
        });
    }

    async loadBrands(): Promise<void> {
        try {
            const response = await this.drugService.getBrands().toPromise();
            if (response?.isSuccess && response.data) {
                this.brands = response.data;
            }
        } catch (error) {
            console.error('Error loading brands:', error);
            this.message.error('Failed to load brands');
        }
    }

    async loadGenericNames(search?: string): Promise<void> {
        try {
            const response = await this.drugService.getGenericNames(search).toPromise();
            if (response?.isSuccess && response.data) {
                this.genericNames = response.data;
            }
        } catch (error) {
            console.error('Error loading generic names:', error);
            this.message.error('Failed to load generic names');
        }
    }

    onGenericNameSearch(value: string): void {
        this.loadGenericNames(value);
    }

    async loadDrug(id: string): Promise<void> {
        this.loading = true;
        try {
            const response = await this.drugService.getDrug(id).toPromise();
            if (response?.isSuccess && response.data) {
                const drug = response.data;
                this.drugForm.patchValue({
                    name: drug.name,
                    strength: drug.strength,
                    genericNameId: drug.genericName.id,
                    brandId: drug.brand.id,
                    type: drug.type,
                    unitPrice: drug.unitPrice,
                    photoPath: drug.photoPath,
                    isActive: drug.isActive
                });
            }
        } catch (error) {
            console.error('Error loading drug:', error);
            this.message.error('Failed to load drug');
        } finally {
            this.loading = false;
        }
    }

    async onSubmit(): Promise<void> {
        if (this.drugForm.valid) {
            this.loading = true;
            const formValue = this.drugForm.value;

            const request = {
                name: formValue.name,
                strength: formValue.strength,
                genericNameId: formValue.genericNameId,
                brandId: formValue.brandId,
                type: formValue.type,
                unitPrice: formValue.unitPrice,
                photoPath: formValue.photoPath,
                isActive: formValue.isActive
            };

            try {
                const operation = this.isEditMode
                    ? this.drugService.updateDrug(this.drugId!, request)
                    : this.drugService.createDrug(request);

                await operation.toPromise();
                this.message.success(`Drug ${this.isEditMode ? 'updated' : 'created'} successfully`);
                await this.router.navigate(['/admin/dispensary/drugs']);
            } catch (error) {
                console.error('Error saving drug:', error);
                this.message.error(`Failed to ${this.isEditMode ? 'update' : 'create'} drug`);
            } finally {
                this.loading = false;
            }
        } else {
            Object.values(this.drugForm.controls).forEach(control => {
                control.markAsDirty();
                control.updateValueAndValidity();
            });
        }
    }

    cancel(): void {
        this.router.navigate(['/admin/dispensary/drugs']);
    }
}
