var flights = JSON.parse('{ "flights": [{ "origin" : "POZ", "destination" : "KUT", "price" : 280, "beginDate" : "2020-02-04 15:00", "endDate" : "2020-02-08 09:00", "route": [{ "departureFrom" : "POZ", "arriveTo" : "WAW", "carrier" : "Wizzair", "flight" : "WZ 1234", "price" : 80, "departure" : "2020-02-04 15:00", "arrival" : "2020-02-04 16:00" }, { "departureFrom" : "WMI", "arriveTo" : "KUT", "carrier" : "Ryanair", "flight" : "R 1234", "price" : 120, "departure" : "2020-02-05 17:30", "arrival" : "2020-02-05 19:00" } ], "return": [{ "departureFrom" : "KUT", "arriveTo" : "POZ", "carrier" : "Wizzair", "flight" : "WZ 1234", "price" : 80, "departure" : "2020-02-08 07:00", "arrival" : "2020-02-08 09:00" }] },{ "origin": "WRO", "destination": "SXF", "price": 200, "beginDate": "2020-02-06 11:00", "endDate": "2020-02-07 14:00", "route": [{ "departureFrom": "WRO", "arriveTo": "SXF", "carrier": "Wizzair", "flight": "WZ 5555", "price": 100, "departure": "2020-02-04 15:00", "arrival": "2020-02-04 16:00" }, { "departureFrom": "WMI", "arriveTo": "KUT", "carrier": "Ryanair", "flight": "R 1234", "price": 120, "departure": "2020-02-05 17:30", "arrival": "2020-02-05 19:00" }], "return": [{ "departureFrom": "KUT", "arriveTo": "POZ", "carrier": "Wizzair", "flight": "WZ 1234", "price": 80, "departure": "2020-02-08 07:00", "arrival": "2020-02-08 09:00" }] }] }');