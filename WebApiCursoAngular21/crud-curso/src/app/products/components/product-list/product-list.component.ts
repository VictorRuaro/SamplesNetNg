import { Component, inject, OnInit, signal, ViewChild, viewChild, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
//interface
import { Product } from '../../interfaces/product';
//service
import { ProductService } from '../../../services/product.service';

//material
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatCard, MatCardContent } from '@angular/material/card';
import { MatDivider } from '@angular/material/divider';
import { Router } from '@angular/router';
import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-product-list',
  imports: [CommonModule, 
    MatTableModule,MatPaginatorModule,
    MatButtonModule,
    MatCard,MatCardContent,
    MatDivider
  ],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent implements OnInit {
  private router = inject(Router)
  private productService = inject(ProductService)
  private dialog = inject(MatDialog)
  // products: Product[] =[];
  @ViewChild(MatPaginator) paginator!: MatPaginator;

   products: WritableSignal<Product[]> = signal<Product[]>([]);

   displayedColumns: string[]=["id","descripcion","stock","actions"]
   dataSource = new MatTableDataSource<Product>([]);

  ngOnInit(): void {
    this.loadProducts();
    /* this.productService.getProducts().subscribe({
      next: (resp)=> {
        console.log('Api response',resp)
      },
      error:(err)=>{
        console.log('Error al obtener produtcs: ', err)
      },
      complete:()=>{

      }
    }) */
    
    }

  loadProducts()
  {
    this.productService.getProducts().subscribe({
      next: (products)=> {
        this.products.set(products)
        this.updateTableData();
      }
    })
  }

  updateTableData(){
    this.dataSource.data = this.products();
    this.dataSource.paginator= this.paginator;
  }

  navigateToForm(id?:number)
  {
    const path=id? `/products/edit/${id}`:`/products/new`;
    this.router.navigate([path]);
  }
  deleteProduct(id:number)
  {
    const dialogRef= this.dialog.open(ConfirmationDialogComponent);

    dialogRef.afterClosed().subscribe((result)=>{
      if(result){
        this.productService.deleteProduct(id).subscribe(()=>{
          const updateProducts = this.products().filter((product)=> product.id !==id)
          this.products.set(updateProducts);
          this.updateTableData();
        })
      }
    })

  }

}
