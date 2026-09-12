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
  readonly validators: ValidatorFn[];
  readonly validationMessages: readonly ValidationMessageConfig[];
}

export const loginFields: Record<LoginFieldName, FieldConfig> = {
  username: {
    name: 'username',
    labelKey: 'login.fields.username.label',
    type: 'text',
    autocomplete: 'username',
    validators: [Validators.required],
    validationMessages: [
      { error: 'required', translationKey: 'validation.login.username.required' },
    ],
  },
  password: {
    name: 'password',
    labelKey: 'login.fields.password.label',
    type: 'password',
    autocomplete: 'current-password',
    validators: [Validators.required],
    validationMessages: [
      { error: 'required', translationKey: 'validation.login.password.required' },
    ],
  },
};
