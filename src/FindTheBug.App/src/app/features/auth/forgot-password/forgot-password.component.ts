import { Component, inject } from '@angular/core';

import { RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzResultModule } from 'ng-zorro-antd/result';
import { AuthService } from '../../../core/services/auth.service';

@Component({
    selector: 'app-forgot-password',
    standalone: true,
    imports: [
    ReactiveFormsModule,
    RouterLink,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzIconModule,
    NzAlertModule,
    NzResultModule
],
    template: `
    <div class="forgot-password-container">
      @if (!isSuccess) {
        <h2 class="form-title">Reset Password</h2>
        <p class="form-subtitle">Enter your email address and we'll send you a link to reset your password.</p>
        @if (errorMessage) {
          <nz-alert
            nzType="error"
            [nzMessage]="errorMessage"
            nzShowIcon
            class="error-alert"
          ></nz-alert>
        }
        <form nz-form [formGroup]="resetForm" class="login-form" (ngSubmit)="submitForm()">
          <nz-form-item>
            <nz-form-control nzErrorTip="Please input a valid email address!">
              <nz-input-group nzPrefixIcon="mail">
                <input type="email" nz-input formControlName="email" placeholder="Email" />
              </nz-input-group>
            </nz-form-control>
          </nz-form-item>
          <button
            nz-button
            class="submit-button"
            [nzType]="'primary'"
            [nzLoading]="isLoading"
            [disabled]="resetForm.invalid"
            >
            Send Reset Link
          </button>
          <div class="back-link">
            <a routerLink="/login">
              <span nz-icon nzType="arrow-left"></span> Back to Login
            </a>
          </div>
        </form>
      } @else {
        <nz-result
          nzStatus="success"
          nzTitle="Check your email"
          nzSubTitle="We have sent a password reset link to your email address."
          >
          <div nz-result-extra>
            <a routerLink="/login">
              <button nz-button nzType="primary">Back to Login</button>
            </a>
          </div>
        </nz-result>
      }
    
    </div>
    `,
  styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {
    private fb = inject(FormBuilder);
    private authService = inject(AuthService);

    resetForm: FormGroup;
    isLoading = false;
    isSuccess = false;
    errorMessage: string | null = null;

    constructor() {
        this.resetForm = this.fb.group({
            email: ['', [Validators.required, Validators.email]]
        });
    }

    async submitForm(): Promise<void> {
        if (this.resetForm.valid) {
            this.isLoading = true;
            this.errorMessage = null;

            try {
                const { email } = this.resetForm.value;
                const response = await this.authService.requestPasswordReset(email);

                if (response.isSuccess) {
                    this.isSuccess = true;
                } else {
                    this.errorMessage = response.errorMessage || 'Failed to send reset link. Please try again.';
                }
            } catch (error) {
                this.errorMessage = 'An unexpected error occurred. Please try again later.';
                console.error('Password reset error:', error);
            } finally {
                this.isLoading = false;
            }
        } else {
            Object.values(this.resetForm.controls).forEach(control => {
                if (control.invalid) {
                    control.markAsDirty();
                    control.updateValueAndValidity({ onlySelf: true });
                }
            });
        }
    }
}