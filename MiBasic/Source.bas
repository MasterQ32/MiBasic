﻿
VARIABLE x Is Integer;

VARIABLE y Is Real;

Variable something Is BOOL;

Structure Datum Is
	member0 Is INTEGER;
	member1 Is REAL;
End

	Function func1();
Function func2(i Is Real);
Function func3(x Is Real, y Is Real);

Function func4() Is Integer;
Function func5(i Is Real) Is Integer;
Function func6(x Is Real, y Is Real) Is Integer;


	Function func(x Is Integer, y Is Integer) Is Integer;
		
VARIABLE c Is Integer;
VARIABLE d Is Integer;

Function implemented()
		local a Is Integer;
		local b Is Integer;
implementation Is
	a = 10 * 30 + 40 - func(10, 20) * (a + b - c * d);
	a = -10;
	a = 10 - -10;
	a = 10 - ~10;
	a = -a;
	a = a + -b;
	a = a + -10;
	a = 10 + 20;
	((b)) = 20;
End