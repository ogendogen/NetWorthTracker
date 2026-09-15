import { ValidatorFn, Validators } from '@angular/forms';

export type LoginFieldName = 'username' | 'password';

export interface ValidationMessageConfig {
  readonly error: string;
  readonly translationKey: string;
}

export interface FieldConfig {
  readonly name: LoginFieldName;
  readonly labelKey: string;
  readonly type: 'text' | 'password';
  readonly autocomplete: string;
  readonly minLength?: number;
  readonly maxLength?: number;
  readonly validators: ValidatorFn[];
  readonly validationMessages: readonly ValidationMessageConfig[];
}

export const loginFields: Record<LoginFieldName, FieldConfig> = {
  username: {
    name: 'username',
    labelKey: 'login.fields.username.label',
    type: 'text',
    autocomplete: 'username',
    minLength: 3,
    maxLength: 32,
    validators: [Validators.required, Validators.minLength(3), Validators.maxLength(32)],
    validationMessages: [
      { error: 'required', translationKey: 'validation.login.username.required' },
      { error: 'minlength', translationKey: 'validation.login.username.minLength' },
      { error: 'maxlength', translationKey: 'validation.login.username.maxLength' },
    ],
  },
  password: {
    name: 'password',
    labelKey: 'login.fields.password.label',
    type: 'password',
    autocomplete: 'current-password',
    minLength: 8,
    maxLength: 64,
    validators: [
      Validators.required,
      Validators.minLength(8),
      Validators.maxLength(64),
    ],
    validationMessages: [
      { error: 'required', translationKey: 'validation.login.password.required' },
      { error: 'minlength', translationKey: 'validation.login.password.minLength' },
      { error: 'maxlength', translationKey: 'validation.login.password.maxLength' },
    ],
  },
};
