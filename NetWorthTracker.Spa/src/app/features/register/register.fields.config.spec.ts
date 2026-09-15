import { FormControl, FormGroup } from '@angular/forms';
import {
  passwordsMatch,
  RegisterFieldName,
  registerFields,
} from './register.fields.config';

const createControl = (field: RegisterFieldName): FormControl<string> =>
  new FormControl('', {
    nonNullable: true,
    validators: registerFields[field].validators,
  });

describe('registerFields', () => {
  it('requires every registration field', () => {
    for (const field of Object.keys(registerFields) as RegisterFieldName[]) {
      expect(createControl(field).hasError('required')).toBe(true);
    }
  });

  describe('username', () => {
    it('requires at least 3 characters', () => {
      const control = createControl('username');

      control.setValue('ab');
      expect(control.hasError('minlength')).toBe(true);

      control.setValue('abc');
      expect(control.valid).toBe(true);
    });

    it('allows no more than 32 characters', () => {
      const control = createControl('username');

      control.setValue('a'.repeat(32));
      expect(control.valid).toBe(true);

      control.setValue('a'.repeat(33));
      expect(control.hasError('maxlength')).toBe(true);
    });
  });

  describe('email', () => {
    it('accepts valid addresses with domain suffixes', () => {
      const control = createControl('email');

      for (const email of ['user@example.com', 'first.last+tag@sub.example.co.uk']) {
        control.setValue(email);
        expect(control.valid).toBe(true);
      }
    });

    it('rejects invalid addresses using the custom validator', () => {
      const control = createControl('email');
      const invalidEmails = [
        'plain-address',
        'user@example',
        'user@example.c',
        'user@@example.com',
        '.user@example.com',
        'user..name@example.com',
        'user@example..com',
        'user @example.com',
      ];

      for (const email of invalidEmails) {
        control.setValue(email);
        expect(control.hasError('email')).toBe(true);
      }
    });

    it('allows no more than 320 characters', () => {
      const control = createControl('email');

      control.setValue(`${'a'.repeat(308)}@example.com`);
      expect(control.valid).toBe(true);

      control.setValue(`${'a'.repeat(309)}@example.com`);
      expect(control.hasError('maxlength')).toBe(true);
    });
  });

  describe('password', () => {
    it('requires at least 8 characters', () => {
      const control = createControl('password');

      control.setValue('Aa1!aaa');
      expect(control.hasError('minlength')).toBe(true);

      control.setValue('Aa1!aaaa');
      expect(control.valid).toBe(true);
    });

    it('allows no more than 64 characters', () => {
      const control = createControl('password');

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
});

describe('passwordsMatch', () => {
  it('accepts matching passwords', () => {
    const form = new FormGroup(
      {
        password: new FormControl('Valid1!Password'),
        confirmPassword: new FormControl('Valid1!Password'),
      },
      { validators: passwordsMatch },
    );

    expect(form.hasError('passwordMismatch')).toBe(false);
  });

  it('rejects mismatched passwords', () => {
    const form = new FormGroup(
      {
        password: new FormControl('Valid1!Password'),
        confirmPassword: new FormControl('Different1!Password'),
      },
      { validators: passwordsMatch },
    );

    expect(form.hasError('passwordMismatch')).toBe(true);
  });
});
