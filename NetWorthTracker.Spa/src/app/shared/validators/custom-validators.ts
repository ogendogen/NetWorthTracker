import { ValidatorFn } from '@angular/forms';
import { EmailWithDomainSuffixValidator } from './email-with-domain-suffix.validator';
import { PasswordComplexityValidator } from './password-complexity.validator';
import { PasswordsMatchValidator } from './passwords-match.validator';

export class CustomValidators {
  static readonly emailWithDomainSuffix: ValidatorFn = EmailWithDomainSuffixValidator.validator;
  static readonly passwordComplexity: ValidatorFn = PasswordComplexityValidator.validator;
  // Group-level validator: expects `password` and `confirmPassword` controls.
  static readonly passwordsMatch: ValidatorFn = PasswordsMatchValidator.validator;

  static readonly all: ValidatorFn[] = [
    CustomValidators.emailWithDomainSuffix,
    CustomValidators.passwordComplexity,
  ];
}
