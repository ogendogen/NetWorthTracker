import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { ForgotPasswordFieldName, forgotPasswordFields } from './forgot-password.fields.config';

@Component({
  selector: 'app-forgot-password-page',
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
  templateUrl: './forgot-password.page.html',
  styleUrl: './forgot-password.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ForgotPasswordPageComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly forgotPasswordFields = forgotPasswordFields;
  readonly form = this.formBuilder.group({
    email: ['', forgotPasswordFields.email.validators],
  });
  readonly isSubmitting = signal(false);
  readonly requestFailed = signal(false);

  getValidationMessage(field: ForgotPasswordFieldName): string {
    const control = this.form.controls[field];
    return (
      forgotPasswordFields[field].validationMessages.find(({ error }) => control.hasError(error))
        ?.translationKey ?? ''
    );
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.requestFailed.set(false);
    this.authService
      .forgotPassword(this.form.getRawValue())
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => void this.router.navigateByUrl('/forgot-password-sent'),
        error: () => this.requestFailed.set(true),
      });
  }
}
