
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

Function implemented()
		implementation Is
	10 * 30 + 40 - func(10, 20) * (a + b - c * d);
	-10;
	10 - -10;
	10 - ~10;
	-a;
	a +-b;
	a +-10;
	a = 10 + 20;
End