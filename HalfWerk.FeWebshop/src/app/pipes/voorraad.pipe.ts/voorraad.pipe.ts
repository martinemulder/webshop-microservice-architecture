import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'voorraad',
  pure: false,
})
export class VoorraadPipe implements PipeTransform {

  transform(value: number): string {
    return `${value > 8 ? 8 : value} op voorraad`;
  }

}
