import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

const passwordComplexityPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9\s]).+$/;

export class PasswordComplexityValidator {
  static readonly validator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    const value = control.value as string;

    if (!value) {
      return null;
    }

    return passwordComplexityPattern.test(value) ? null : { pattern: true };
  };
}
