import {
  AbstractControl,
  ValidationErrors,
  ValidatorFn,
} from '@angular/forms';

const emailLocalPartPattern = /^[^@\s.]+(?:\.[^@\s.]+)*$/;
const emailDomainSuffixPattern = /^(?:[^@\s.]+\.)+[^@\s.]{2,}$/;

export class EmailWithDomainSuffixValidator {
  static readonly validator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    const value = control.value as string;

    if (!value) {
      return null;
    }

    const parts = value.split('@');
    const isValid =
      parts.length === 2 &&
      emailLocalPartPattern.test(parts[0]) &&
      emailDomainSuffixPattern.test(parts[1]);

    return isValid ? null : { email: true };
  };
}
