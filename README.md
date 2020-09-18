# Aeroradar
Webscraper - wyszukiwarka połączeń lotniczych.
Projekt zespołowy z przedmiotu Podstawy Teleinformatyki.

## Dokumentacja projektu
Dokumentacja jest dostępna pod poniższym linkiem:
https://drive.google.com/file/d/1Uupa-yWgEDPmncA-ckJ_JrqVQMDAgDfl/

## Opis aplikacji
Program Aeroradar jest wyszukiwarką połączeń lotniczych pozwalającą wyszukać loty pomiędzy wskazanymi lotniskami w konkretnym dniu. Aplikacja korzysta z *oscrapowanych* danych ze stron linii **Wizzair**, **Ryanair** oraz **Transavia**, a także pobiera informacjach o kodach i nazwach lotnisk ze strony organizacji **IATA** *(iata.org)*. 

## Instrukcja korzystania z programu
Po uruchomieniu programu należy korzystając z panelu znajdującego się po lewej stronie aplikacji wybrać dzień oraz lotniska wylotu i przylotu. Co istotne, pola lotnisk są wyszukiwarkami, co oznacza, że możemy tam wprowadzić zarówno kod jak i miasto czy nazwę lotniska (w języku angielskim). Finalnie, wartość w polu tekstowym musi być wybrana z jednej z dostarczonych propozycji.
Po wprowadzeniu danych, należy kliknąć w przycisk **WYSZUKAJ** i zaczekać kilka sekund. To wszystko!

## Uwagi
Ze względu na zastosowane zabezpieczenia antybotowe stron przewoźników, dane przewoźnika **Wizzair** dostarczane są z około 10-sekundowym opóźnieniem, natomiast możliwość wyszukiwania lotów przewoźnika **Ryanair** jest ograniczona do kilku, po czym IP klienta jest na jakiś czas blokowane.

## Przykładowe dane
Zapytanie zwracające dwa loty - **Wizzair** i **Ryanair**:
```
2020-07-22 [VIE] Vienna Schwechat Intl - [DTM] Dortmund Airport
```

Zapytanie zwracające lot przewoźnika **Transavia**:
```
2020-08-25 [EIN] Eindhoven Airport - [ACE] Lanzarote Lanzarote
```

Zapytanie zwracające dwa loty przewoźnika w jednym dniu:
```
2020-09-12 [EIN] Eindhoven Airport - [BCN] Barcelona Airport
```
