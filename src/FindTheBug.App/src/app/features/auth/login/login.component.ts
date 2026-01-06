import { Component, inject } from '@angular/core';

import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzCheckboxModule,
    NzIconModule,
    NzAlertModule
],
  template: `
    <div class="login-container">
      <h2 class="form-title">Sign In</h2>
    
      @if (errorMessage) {
        <nz-alert
          nzType="error"
          [nzMessage]="errorMessage"
          nzShowIcon
          class="error-alert"
        ></nz-alert>
      }
    
      <form nz-form [formGroup]="loginForm" class="login-form" (ngSubmit)="submitForm()">
        <nz-form-item>
          <nz-form-control nzErrorTip="Please input your email!">
            <nz-input-group nzPrefixIcon="user">
              <input type="email" nz-input formControlName="email" placeholder="Email" />
            </nz-input-group>
          </nz-form-control>
        </nz-form-item>
    
        <nz-form-item>
          <nz-form-control nzErrorTip="Please input your password!">
            <nz-input-group nzPrefixIcon="lock" [nzSuffix]="passwordTemplate">
              <input
                [type]="passwordVisible ? 'text' : 'password'"
                nz-input
                formControlName="password"
                placeholder="Password"
                />
              </nz-input-group>
              <ng-template #passwordTemplate>
                <span
                  nz-icon
                  [nzType]="passwordVisible ? 'eye-invisible' : 'eye'"
                  (click)="passwordVisible = !passwordVisible"
                ></span>
              </ng-template>
            </nz-form-control>
          </nz-form-item>
    
          <div nz-row class="login-form-margin">
            <div nz-col [nzSpan]="12">
              <label nz-checkbox formControlName="remember">Remember me</label>
            </div>
            <div nz-col [nzSpan]="12" class="login-form-forgot">
              <a routerLink="/forgot-password">Forgot password?</a>
            </div>
          </div>
    
          <button
            nz-button
            class="login-form-button"
            [nzType]="'primary'"
            [nzLoading]="isLoading"
            [disabled]="loginForm.invalid"
            >
            Log in
          </button>
        </form>
      </div>
    `,
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  loginForm: FormGroup;
  isLoading = false;
  passwordVisible = false;
  errorMessage: string | null = null;

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
      remember: [true]
    });
  }

  async submitForm(): Promise<void> {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = null;

      try {
        const { email, password } = this.loginForm.value;
        const response = await this.authService.login(email, password);

        if (response.isSuccess) {
          // Get the returnUrl from query params or default to admin dashboard
          const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/admin/dashboard';
          await this.router.navigate([returnUrl]);
        } else {
          this.errorMessage = response.errorMessage || 'Login failed. Please check your credentials.';
        }
      } catch (error) {
        this.errorMessage = 'An unexpected error occurred. Please try again later.';
        console.error('Login error:', error);
      } finally {
        this.isLoading = false;
      }
    } else {
      Object.values(this.loginForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
    }
  }
}