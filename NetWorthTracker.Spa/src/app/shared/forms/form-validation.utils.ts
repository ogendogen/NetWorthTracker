import { AbstractControl } from '@angular/forms';

export interface ValidationMessage {
  readonly error: string;
  readonly translationKey: string;
}

export function getValidationMessage(
  control: AbstractControl,
  messages: readonly ValidationMessage[],
): string {
  return messages.find(({ error }) => control.hasError(error))?.translationKey ?? '';
}
