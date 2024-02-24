import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewItemFormComponent } from './new-item-form.component';

describe('NewItemFormComponent', () => {
  let component: NewItemFormComponent;
  let fixture: ComponentFixture<NewItemFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewItemFormComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(NewItemFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
