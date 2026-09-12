import {
  AbstractControl,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';

export type RegisterFieldName = 'username' | 'email' | 'password' | 'confirmPassword';

export interface ValidationMessageConfig {
  readonly error: string;
  readonly translationKey: string;
}

export interface RegisterFieldConfig {
  readonly name: RegisterFieldName;
  readonly validators: ValidatorFn[];
  readonly validationMessages: readonly ValidationMessageConfig[];
}

export const registerFields: Record<RegisterFieldName, RegisterFieldConfig> = {
  username: {
    name: 'username',
    validators: [Validators.required, Validators.maxLength(32)],
    validationMessages: [
      { error: 'required', translationKey: 'validation.register.username.required' },
      { error: 'maxlength', translationKey: 'validation.register.username.maxLength' },
    ],
  },
  email: {
    name: 'email',
    validators: [Validators.required, Validators.email, Validators.maxLength(320)],
    validationMessages: [
      { error: 'required', translationKey: 'validation.register.email.required' },
      { error: 'email', translationKey: 'validation.register.email.invalid' },
      { error: 'maxlength', translationKey: 'validation.register.email.maxLength' },
    ],
  },
  password: {
    name: 'password',
    validators: [Validators.required],
    validationMessages: [
      { error: 'required', translationKey: 'validation.register.password.required' },
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
