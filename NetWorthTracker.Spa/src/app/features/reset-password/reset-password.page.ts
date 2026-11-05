import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import {
  ResetPasswordFieldName,
  resetPasswordFields,
  resetPasswordFormValidations,
} from './reset-password.fields.config';

@Component({
  selector: 'app-reset-password-page',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    TranslatePipe,
  ],
  templateUrl: './reset-password.page.html',
  styleUrl: './reset-password.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ResetPasswordPageComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly resetPasswordFields = resetPasswordFields;
  readonly resetPasswordFormValidations = resetPasswordFormValidations;
  readonly token = this.route.snapshot.queryParamMap.get('token') ?? '';
  readonly hasToken = this.token.length > 0;
  readonly form = this.formBuilder.group(
    {
      password: ['', resetPasswordFields.password.validators],
      confirmPassword: ['', resetPasswordFields.confirmPassword.validators],
    },
    { validators: resetPasswordFormValidations.passwordMismatch.validator },
  );
  readonly isSubmitting = signal(false);
  readonly resetFailed = signal(false);

  getValidationMessage(field: ResetPasswordFieldName): string {
    const control = this.form.controls[field];
    return (
      resetPasswordFields[field].validationMessages.find(({ error }) => control.hasError(error))
        ?.translationKey ?? ''
    );
  }

  get passwordMismatchVisible(): boolean {
    return (
      this.form.hasError('passwordMismatch') &&
      this.form.controls.password.touched &&
      this.form.controls.confirmPassword.touched
    );
  }

  submit(): void {
    if (!this.hasToken) {
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { password, confirmPassword } = this.form.getRawValue();

    this.isSubmitting.set(true);
    this.resetFailed.set(false);
    this.authService
      .resetPassword({ token: this.token, password, confirmPassword })
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (response) => {
          if (response.success) {
            void this.router.navigateByUrl('/password-reset-success');
          } else {
            this.resetFailed.set(true);
          }
        },
        error: () => this.resetFailed.set(true),
      });
  }
}
