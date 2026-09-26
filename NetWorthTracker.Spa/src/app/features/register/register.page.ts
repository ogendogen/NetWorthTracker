import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TranslatePipe } from '@ngx-translate/core';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import {
  registerFields,
  registerFormValidations,
  RegisterFieldName,
} from './register.fields.config';

@Component({
  selector: 'app-register-page',
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
  templateUrl: './register.page.html',
  styleUrl: './register.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegisterPageComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly registerFields = registerFields;
  readonly form = this.formBuilder.group(
    {
      username: ['', registerFields.username.validators],
      email: ['', registerFields.email.validators],
      password: ['', registerFields.password.validators],
      confirmPassword: ['', registerFields.confirmPassword.validators],
    },
    { validators: registerFormValidations.passwordMismatch.validator },
  );
  readonly isSubmitting = signal(false);
  readonly registrationFailed = signal(false);

  getValidationMessage(field: RegisterFieldName): string {
    const control = this.form.controls[field];
    return (
      registerFields[field].validationMessages.find(({ error }) => control.hasError(error))
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
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { username, email, password } = this.form.getRawValue();

    this.isSubmitting.set(true);
    this.registrationFailed.set(false);
    this.authService
      .register({ username, email, password })
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (response) => {
          if (response.success) {
            void this.router.navigateByUrl('/registration-success');
          } else {
            this.registrationFailed.set(true);
          }
        },
        error: () => this.registrationFailed.set(true),
      });
  }
}
