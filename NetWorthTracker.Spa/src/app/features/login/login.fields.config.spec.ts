import { FormControl } from '@angular/forms';
import { LoginFieldName, loginFields } from './login.fields.config';

const createControl = (field: LoginFieldName): FormControl<string> =>
  new FormControl('', {
    nonNullable: true,
    validators: loginFields[field].validators,
  });

describe('loginFields', () => {
  it('requires both login fields', () => {
    expect(createControl('username').hasError('required')).toBe(true);
    expect(createControl('password').hasError('required')).toBe(true);
  });

  it('enforces a username between 3 and 32 characters', () => {
    const control = createControl('username');

    control.setValue('ab');
    expect(control.hasError('minlength')).toBe(true);

    control.setValue('a'.repeat(3));
    expect(control.valid).toBe(true);

    control.setValue('a'.repeat(32));
    expect(control.valid).toBe(true);

    control.setValue('a'.repeat(33));
    expect(control.hasError('maxlength')).toBe(true);
  });

  it('enforces a password between 8 and 64 characters', () => {
    const control = createControl('password');

    control.setValue('Aa1!aaa');
    expect(control.hasError('minlength')).toBe(true);

    control.setValue('Aa1!aaaa');
    expect(control.valid).toBe(true);

    control.setValue(`Aa1!${'a'.repeat(60)}`);
    expect(control.valid).toBe(true);

    control.setValue(`Aa1!${'a'.repeat(61)}`);
    expect(control.hasError('maxlength')).toBe(true);
  });

  it('requires uppercase, lowercase, number, and special-symbol characters', () => {
    const control = createControl('password');
    const invalidPasswords = [
      'lowercase1!',
      'UPPERCASE1!',
      'NoNumber!',
      'NoSymbol1',
      'Aa1 aaaa',
    ];

    for (const password of invalidPasswords) {
      control.setValue(password);
      expect(control.hasError('pattern')).toBe(true);
    }

    control.setValue('Valid1!Password');
    expect(control.valid).toBe(true);
  });
});
