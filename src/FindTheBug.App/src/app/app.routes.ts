import { Routes } from '@angular/router';
import { AuthLayoutComponent } from './features/auth/auth-layout/auth-layout.component';
import { LoginComponent } from './features/auth/login/login.component';
import { ForgotPasswordComponent } from './features/auth/forgot-password/forgot-password.component';
import { AdminLayoutComponent } from './features/admin/admin-layout/admin-layout.component';
import { DashboardComponent } from './features/admin/dashboard/dashboard.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    {
        path: '',
        component: AuthLayoutComponent,
        children: [
            { path: 'login', component: LoginComponent },
            { path: 'forgot-password', component: ForgotPasswordComponent },
            { path: '', redirectTo: 'login', pathMatch: 'full' }
        ]
    },
    {
        path: 'admin',
        component: AdminLayoutComponent,
        canActivate: [authGuard],
        children: [
            {
                path: '',
                redirectTo: 'dashboard',
                pathMatch: 'full'
            },
            {
                path: 'dashboard',
                component: DashboardComponent
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
            // Future routes can be added here
            // { path: 'patients', loadComponent: () => import('./features/admin/patients/patients.component').then(c => c.PatientsComponent) },
            // { path: 'tests', loadComponent: () => import('./features/admin/tests/tests.component').then(c => c.TestsComponent) },
        ]
    },
    { path: '**', redirectTo: 'login' }
];
