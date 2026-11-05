import { ValidatorFn, Validators } from '@angular/forms';
import { CustomValidators } from '../../shared/validators/custom-validators';

export type ForgotPasswordFieldName = 'email';

export interface ValidationMessageConfig {
  readonly error: string;
  readonly translationKey: string;
}

export interface ForgotPasswordFieldConfig {
  readonly name: ForgotPasswordFieldName;
  readonly maxLength?: number;
  readonly validators: ValidatorFn[];
  readonly validationMessages: readonly ValidationMessageConfig[];
}

export const forgotPasswordFields: Record<ForgotPasswordFieldName, ForgotPasswordFieldConfig> = {
  email: {
    name: 'email',
    maxLength: 320,
    validators: [
      Validators.required,
      CustomValidators.emailWithDomainSuffix,
      Validators.maxLength(320),
    ],
    validationMessages: [
      { error: 'required', translationKey: 'validation.forgotPassword.email.required' },
      { error: 'email', translationKey: 'validation.forgotPassword.email.invalid' },
      { error: 'maxlength', translationKey: 'validation.forgotPassword.email.maxLength' },
    ],
  },
};
