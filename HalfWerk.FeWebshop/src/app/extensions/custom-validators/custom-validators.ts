import { AbstractControl } from '@angular/forms';

export class CustomValidators {
  static passwordMatchValidator(control: AbstractControl) {
    const password: string = control.get('wachtwoord').value;
    const confirmPassword: string = control.get('bevestigWachtwoord').value;

    if (password !== confirmPassword) {
      control.get('bevestigWachtwoord').setErrors({ NoPassswordMatch: true });
    }
  }
}
