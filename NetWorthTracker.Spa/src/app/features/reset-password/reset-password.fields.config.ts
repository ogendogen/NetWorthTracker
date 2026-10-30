import { ValidatorFn, Validators } from '@angular/forms';
import { CustomValidators } from '../../shared/validators/custom-validators';

export type ResetPasswordFieldName = 'password' | 'confirmPassword';

export interface ValidationMessageConfig {
  readonly error: string;
  readonly translationKey: string;
}

export interface ResetPasswordFieldConfig {
  readonly name: ResetPasswordFieldName;
  readonly minLength?: number;
  readonly maxLength?: number;
  readonly validators: ValidatorFn[];
  readonly validationMessages: readonly ValidationMessageConfig[];
}

export const resetPasswordFields: Record<ResetPasswordFieldName, ResetPasswordFieldConfig> = {
  password: {
    name: 'password',
    minLength: 8,
    maxLength: 64,
    validators: [
      Validators.required,
      Validators.minLength(8),
      Validators.maxLength(64),
      CustomValidators.passwordComplexity,
    ],
    validationMessages: [
      { error: 'required', translationKey: 'validation.resetPassword.password.required' },
      { error: 'minlength', translationKey: 'validation.resetPassword.password.minLength' },
      { error: 'maxlength', translationKey: 'validation.resetPassword.password.maxLength' },
      { error: 'pattern', translationKey: 'validation.resetPassword.password.complexity' },
    ],
  },
  confirmPassword: {
    name: 'confirmPassword',
    validators: [Validators.required],
    validationMessages: [
      { error: 'required', translationKey: 'validation.resetPassword.confirmPassword.required' },
    ],
  },
};

export const resetPasswordFormValidations = {
  passwordMismatch: {
    validator: CustomValidators.passwordsMatch,
    translationKey: 'validation.resetPassword.passwordMismatch',
  },
};
