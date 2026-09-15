import {
  AbstractControl,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { CustomValidators } from '../../shared/validators/custom-validators';

export type RegisterFieldName = 'username' | 'email' | 'password' | 'confirmPassword';

export interface ValidationMessageConfig {
  readonly error: string;
  readonly translationKey: string;
}

export interface RegisterFieldConfig {
  readonly name: RegisterFieldName;
  readonly minLength?: number;
  readonly maxLength?: number;
  readonly validators: ValidatorFn[];
  readonly validationMessages: readonly ValidationMessageConfig[];
}

export const registerFields: Record<RegisterFieldName, RegisterFieldConfig> = {
  username: {
    name: 'username',
    minLength: 3,
    maxLength: 32,
    validators: [Validators.required, Validators.minLength(3), Validators.maxLength(32)],
    validationMessages: [
      { error: 'required', translationKey: 'validation.register.username.required' },
      { error: 'minlength', translationKey: 'validation.register.username.minLength' },
      { error: 'maxlength', translationKey: 'validation.register.username.maxLength' },
    ],
  },
  email: {
    name: 'email',
    validators: [
      Validators.required,
      CustomValidators.emailWithDomainSuffix,
      Validators.maxLength(320),
    ],
    validationMessages: [
      { error: 'required', translationKey: 'validation.register.email.required' },
      { error: 'email', translationKey: 'validation.register.email.invalid' },
      { error: 'maxlength', translationKey: 'validation.register.email.maxLength' },
    ],
  },
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
      { error: 'required', translationKey: 'validation.register.password.required' },
      { error: 'minlength', translationKey: 'validation.register.password.minLength' },
      { error: 'maxlength', translationKey: 'validation.register.password.maxLength' },
      { error: 'pattern', translationKey: 'validation.register.password.complexity' },
    ],
  },
  confirmPassword: {
    name: 'confirmPassword',
    validators: [Validators.required],
    validationMessages: [
      { error: 'required', translationKey: 'validation.register.confirmPassword.required' },
    ],
  },
};

export const passwordsMatch: ValidatorFn = (control: AbstractControl): ValidationErrors | null =>
  control.get('password')?.value === control.get('confirmPassword')?.value
    ? null
    : { passwordMismatch: true };

export const registerFormValidations = {
  passwordMismatch: {
    validator: passwordsMatch,
    translationKey: 'validation.register.passwordMismatch',
  },
};
