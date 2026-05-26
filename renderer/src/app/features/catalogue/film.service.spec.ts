import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { FilmService } from './film.service';

describe('FilmService', () => {
  let service: FilmService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), FilmService],
    });
    service = TestBed.inject(FilmService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('lister() appelle GET /api/films', () => {
    service.lister().subscribe();
    const req = httpMock.expectOne('/api/films');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });
});
