import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzCardModule } from 'ng-zorro-antd/card';
import { ProductService } from '../../../../../core/services/product.service';
import { Product, CreateProductRequest, UpdateProductRequest } from '../../../../../core/models/product.models';

@Component({
    selector: 'app-product-form',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        NzFormModule,
        NzInputModule,
        NzButtonModule,
        NzInputNumberModule,
        NzSwitchModule,
        NzCardModule
    ],
    templateUrl: './product-form.component.html',
    styleUrls: ['./product-form.component.css']
})
export class ProductFormComponent implements OnInit {
    private readonly fb = inject(FormBuilder);
    private readonly productService = inject(ProductService);
    private readonly router = inject(Router);
    private readonly route = inject(ActivatedRoute);
    private readonly message = inject(NzMessageService);

    productForm!: FormGroup;
    isEditMode = false;
    loading = false;
    productId?: string;

    ngOnInit(): void {
        this.productForm = this.fb.group({
            name: ['', [Validators.required]],
            price: [0, [Validators.required, Validators.min(0)]],
            quantity: [1, [Validators.required, Validators.min(0)]],
            description: [''],
            photoPath: [''],
            isActive: [true]
        });

        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.isEditMode = true;
            this.productId = id;
            this.loadProduct(id);
        }
    }

    async loadProduct(id: string): Promise<void> {
        this.loading = true;
        try {
            const response = await this.productService.getProduct(id).toPromise();
            if (response?.isSuccess && response.data) {
                const product = response.data;
                this.productForm.patchValue({
                    name: product.name,
                    price: product.price,
                    quantity: product.quantity,
                    description: product.description || '',
                    photoPath: product.photoPath || '',
                    isActive: product.isActive
                });
            }
        } catch (error) {
            console.error('Error loading product:', error);
            this.message.error('Failed to load product');
        } finally {
            this.loading = false;
        }
    }

    async onSubmit(): Promise<void> {
        if (this.productForm.invalid) {
            Object.values(this.productForm.controls).forEach(control => {
                if (control.invalid) {
                    control.markAsDirty();
                    control.updateValueAndValidity();
                }
            });
            return;
        }

        this.loading = true;
        try {
            const formValue = this.productForm.value;
            
            if (this.isEditMode && this.productId) {
                const updateRequest: UpdateProductRequest = {
                    name: formValue.name,
                    price: formValue.price,
                    quantity: formValue.quantity,
                    description: formValue.description,
                    photoPath: formValue.photoPath,
                    isActive: formValue.isActive
                };
                
                await this.productService.updateProduct(this.productId, updateRequest).toPromise();
                this.message.success('Product updated successfully');
            } else {
                const createRequest: CreateProductRequest = {
                    name: formValue.name,
                    price: formValue.price,
                    quantity: formValue.quantity,
                    description: formValue.description,
                    photoPath: formValue.photoPath
                };
                
                await this.productService.createProduct(createRequest).toPromise();
                this.message.success('Product created successfully');
            }
            
            this.router.navigate(['/admin/dispensary/products']);
        } catch (error) {
            console.error('Error saving product:', error);
            this.message.error('Failed to save product');
        } finally {
            this.loading = false;
        }
    }

    cancel(): void {
        this.router.navigate(['/admin/dispensary/products']);
    }
}
