import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import('./features/auth/auth-layout/auth-layout.component').then(c => c.AuthLayoutComponent),
        children: [
            { path: 'login', loadComponent: () => import('./features/auth/login/login.component').then(c => c.LoginComponent) },
            { path: 'forgot-password', loadComponent: () => import('./features/auth/forgot-password/forgot-password.component').then(c => c.ForgotPasswordComponent) },
            { path: '', redirectTo: 'login', pathMatch: 'full' }
        ]
    },
    {
        path: 'admin',
        loadComponent: () => import('./features/admin/admin-layout/admin-layout.component').then(c => c.AdminLayoutComponent),
        canActivate: [authGuard],
        children: [
            {
                path: '',
                redirectTo: 'dashboard',
                pathMatch: 'full'
            },
            {
                path: 'dashboard',
                loadComponent: () => import('./features/admin/dashboard/dashboard.component').then(c => c.DashboardComponent)
            },
            {
                path: 'users',
                loadComponent: () => import('./features/admin/users/users-list/users-list.component').then(c => c.UsersListComponent)
            },
            {
                path: 'users/new',
                loadComponent: () => import('./features/admin/users/user-form/user-form.component').then(c => c.UserFormComponent)
            },
            {
                path: 'users/:id/edit',
                loadComponent: () => import('./features/admin/users/user-form/user-form.component').then(c => c.UserFormComponent)
            },
            {
                path: 'roles',
                loadComponent: () => import('./features/admin/roles/roles-list/roles-list.component').then(c => c.RolesListComponent)
            },
            {
                path: 'roles/create',
                loadComponent: () => import('./features/admin/roles/role-form/role-form.component').then(c => c.RoleFormComponent)
            },
            {
                path: 'roles/:id/edit',
                loadComponent: () => import('./features/admin/roles/role-form/role-form.component').then(c => c.RoleFormComponent)
            },
            {
                path: 'doctors',
                loadComponent: () => import('./features/admin/doctors/doctors-list/doctors-list.component').then(c => c.DoctorsListComponent)
            },
            {
                path: 'doctors/create',
                loadComponent: () => import('./features/admin/doctors/doctor-form/doctor-form.component').then(c => c.DoctorFormComponent)
            },
            {
                path: 'doctors/:id',
                loadComponent: () => import('./features/admin/doctors/doctor-form/doctor-form.component').then(c => c.DoctorFormComponent)
            },
            {
                path: 'doctors/:id/edit',
                loadComponent: () => import('./features/admin/doctors/doctor-form/doctor-form.component').then(c => c.DoctorFormComponent)
            },
            {
                path: 'dispensary/drugs',
                loadComponent: () => import('./features/admin/dispensary/drugs/drugs-list/drugs-list.component').then(c => c.DrugsListComponent)
            },
            {
                path: 'dispensary/drugs/create',
                loadComponent: () => import('./features/admin/dispensary/drugs/drug-form/drug-form.component').then(c => c.DrugFormComponent)
            },
            {
                path: 'dispensary/drugs/:id/edit',
                loadComponent: () => import('./features/admin/dispensary/drugs/drug-form/drug-form.component').then(c => c.DrugFormComponent)
            },
            {
                path: 'dispensary/products',
                loadComponent: () => import('./features/admin/dispensary/products/products-list/products-list.component').then(c => c.ProductsListComponent)
            },
            {
                path: 'dispensary/products/create',
                loadComponent: () => import('./features/admin/dispensary/products/product-form/product-form.component').then(c => c.ProductFormComponent)
            },
            {
                path: 'dispensary/products/:id/edit',
                loadComponent: () => import('./features/admin/dispensary/products/product-form/product-form.component').then(c => c.ProductFormComponent)
            },
            {
                path: 'dispensary/expenses',
                data: { department: 'Dispensary' },
                loadComponent: () => import('./features/admin/dispensary/expenses/expenses-list/expenses-list.component').then(c => c.ExpensesListComponent)
            },
            {
                path: 'dispensary/expenses/create',
                loadComponent: () => import('./features/admin/dispensary/expenses/expense-form/expense-form.component').then(c => c.ExpenseFormComponent)
            },
            {
                path: 'dispensary/expenses/:id/edit',
                loadComponent: () => import('./features/admin/dispensary/expenses/expense-form/expense-form.component').then(c => c.ExpenseFormComponent)
            },
            {
                path: 'transactions/expenses',
                data: { department: 'Lab' },
                loadComponent: () => import('./features/admin/dispensary/expenses/expenses-list/expenses-list.component').then(c => c.ExpensesListComponent)
            },
            {
                path: 'transactions/expenses/create',
                loadComponent: () => import('./features/admin/dispensary/expenses/expense-form/expense-form.component').then(c => c.ExpenseFormComponent)
            },
            {
                path: 'transactions/expenses/:id/edit',
                loadComponent: () => import('./features/admin/dispensary/expenses/expense-form/expense-form.component').then(c => c.ExpenseFormComponent)
            },
            // Future routes can be added here
            // { path: 'patients', loadComponent: () => import('./features/admin/patients/patients.component').then(c => c.PatientsComponent) },
            // { path: 'tests', loadComponent: () => import('./features/admin/tests/tests.component').then(c => c.TestsComponent) },
        ]
    },
    { path: '**', redirectTo: 'login' }
];