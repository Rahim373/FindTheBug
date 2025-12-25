import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { ProductService } from '../../../../../core/services/product.service';
import { ProductListItem } from '../../../../../core/models/product.models';

@Component({
    selector: 'app-products-list',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        NzTableModule,
        NzButtonModule,
        NzInputModule,
        NzIconModule,
        NzModalModule,
        NzTagModule
    ],
    templateUrl: './products-list.component.html',
    styleUrls: ['./products-list.component.css']
})
export class ProductsListComponent implements OnInit {
    private readonly productService = inject(ProductService);
    private readonly router = inject(Router);
    private readonly modal = inject(NzModalService);
    private readonly message = inject(NzMessageService);

    products: ProductListItem[] = [];
    loading = false;
    searchText = '';

    // Pagination
    pageNumber = 1;
    pageSize = 10;
    totalCount = 0;

    ngOnInit(): void {
        this.loadProducts();
    }

    async loadProducts(): Promise<void> {
        this.loading = true;
        try {
            const response = await this.productService.getProducts(this.searchText || undefined, this.pageNumber, this.pageSize).toPromise();
            if (response?.isSuccess && response.data) {
                this.products = response.data.items || [];
                this.totalCount = response.data.totalCount || 0;
            }
        } catch (error) {
            console.error('Error loading products:', error);
            this.message.error('Failed to load products');
        } finally {
            this.loading = false;
        }
    }

    onSearch(): void {
        this.pageNumber = 1;
        this.loadProducts();
    }

    onPageChange(pageNumber: number): void {
        this.pageNumber = pageNumber;
        this.loadProducts();
    }

    onPageSizeChange(pageSize: number): void {
        this.pageSize = pageSize;
        this.pageNumber = 1;
        this.loadProducts();
    }

    createProduct(): void {
        this.router.navigate(['/admin/dispensary/products/create']);
    }

    editProduct(id: string): void {
        this.router.navigate(['/admin/dispensary/products', id, 'edit']);
    }

    deleteProduct(product: ProductListItem): void {
        this.modal.confirm({
            nzTitle: 'Delete Product',
            nzContent: `Are you sure you want to delete ${product.name}?`,
            nzOkText: 'Delete',
            nzOkDanger: true,
            nzOnOk: async () => {
                try {
                    await this.productService.deleteProduct(product.id).toPromise();
                    this.message.success('Product deleted successfully');
                    await this.loadProducts();
                } catch (error) {
                    console.error('Error deleting product:', error);
                    this.message.error('Failed to delete product');
                }
            }
        });
    }
}
