import { ValidatorFn } from '@angular/forms';
import { EmailWithDomainSuffixValidator } from './email-with-domain-suffix.validator';
import { PasswordComplexityValidator } from './password-complexity.validator';

export class CustomValidators {
  static readonly emailWithDomainSuffix: ValidatorFn = EmailWithDomainSuffixValidator.validator;
  static readonly passwordComplexity: ValidatorFn = PasswordComplexityValidator.validator;

  static readonly all: ValidatorFn[] = [
    CustomValidators.emailWithDomainSuffix,
    CustomValidators.passwordComplexity,
  ];
}
