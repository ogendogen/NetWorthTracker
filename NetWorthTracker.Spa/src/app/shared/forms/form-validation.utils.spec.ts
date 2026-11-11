import { FormControl } from '@angular/forms';
import { getValidationMessage, ValidationMessage } from './form-validation.utils';

describe('getValidationMessage', () => {
  const messages: readonly ValidationMessage[] = [
    { error: 'required', translationKey: 'validation.required' },
    { error: 'minlength', translationKey: 'validation.minLength' },
  ];

  it('returns the first matching message in configuration order', () => {
    const control = new FormControl('');
    control.setErrors({ required: true, minlength: true });

    expect(getValidationMessage(control, messages)).toBe('validation.required');
  });

  it('returns the matching message even when another error is present', () => {
    const control = new FormControl('x');
    control.setErrors({ custom: true, minlength: true });

    expect(getValidationMessage(control, messages)).toBe('validation.minLength');
  });

  it('returns an empty string when the control has no errors', () => {
    const control = new FormControl('valid');

    expect(getValidationMessage(control, messages)).toBe('');
  });

  it('returns an empty string when no configured message matches', () => {
    const control = new FormControl('x');
    control.setErrors({ custom: true });

    expect(getValidationMessage(control, messages)).toBe('');
  });
});
