Para desinstalar Node.js de un proyecto de Angular, debes eliminar la carpeta de dependencias y el archivo de bloqueo utilizando la terminal.

Los comandos clave para limpiar tu proyecto son:

Eliminar la carpeta de módulos (Node Modules):
-En Mac/Linux: rm -rf node_modules
-En Windows (PowerShell): Remove-Item -Recurse Forcenode_modules

Eliminar el archivo de bloqueo (borra el historial de caché del proyecto):

rm package-lock.json

Reinstalar desde cero (vuelve a instalar solo las dependencias limpias):

npm install

Si te refieres a desinstalar Angular CLI de tu entorno global de Node, debes usar:

npm uninstall -g @angular/cli

Para asegurarte de que no quedan rastros, puedes verificar la caché con npm cache verify.

--INSTALACION DE ANGULAR MATERIAL--

Ir a la url de angular material https://material.angular.dev/

-seleccionar la version 
-comenzar (Get started)

Agregar angular Material al proyecto

ng add @angular/material

seguir la guia del sitio para agregar los componentes a los componentes del proyecto

el cualquier componente.ts agregar el import

 import {MatButtonModule} from '@angular/material/button';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,
    ProductListComponent,
    MatButtonModule,
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'crud-curso';
}

agregar el tag del componente en el componente.html

<div class="example-button-row">
        <button mat-raised-button>Basic</button>
        <button mat-raised-button disabled>Disabled</button>
        <a mat-raised-button href="https://www.google.com/" target="_blank">Link</a>
      </div>
