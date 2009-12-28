/*
	System Supported Operators
	
	� Copyright 2007 Alphora
*/
create or replace function DAE_Frac (AValue IN NUMERIC)
  RETURNS NUMERIC
  AS $$ declare
    LReturnVal NUMERIC(28, 8);
  BEGIN
    LReturnVal := AValue - TRUNC(AValue, 0);
    RETURN(LReturnVal);
  END;
  $$ LANGUAGE plpgsql;
\
create or replace function DAE_LogB (AValue in NUMERIC, ABase in NUMERIC)
  RETURNS NUMERIC
  AS $$ declare
    LReturnVal NUMERIC(28, 8);
  BEGIN
    LReturnVal := LN(AValue) / LN(ABase);
    RETURN (LReturnVal);
  END;
  $$ LANGUAGE plpgsql;
\
create or replace function DAE_Random()
  RETURNS NUMERIC
  AS $$ declare
    LReturnVal NUMERIC(28,8);
  BEGIN
    LReturnVal := 0;
    --LReturnVal := DAE_Frac(ABS(DBMS_RANDOM.RANDOM) / 100000000);
    --DBMS_RANDOM.TERMINATE;
    RETURN LReturnVal;
  END;
  $$ LANGUAGE plpgsql;
\
create or replace function DAE_Factorial(AValue in int)
  RETURNS NUMERIC
  AS $$ declare 
    LReturnVal NUMERIC(28,8);
  begin
    LReturnVal := 1;
    for i in 1..AValue loop
      LReturnVal := LReturnVal * i;
    end loop;
    RETURN LReturnVal;
  end;
  $$ LANGUAGE plpgsql;
\
create or replace function DAE_TSReadSecond(ATimeSpan in NUMERIC)
	RETURNS NUMERIC
	AS $$ declare
		LReturnVal NUMERIC(20,0);
	begin
		LReturnVal := TRUNC(DAE_Frac(ATimeSpan / (10000000 * 60)) * 60);
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_TSReadMinute(ATimeSpan in NUMERIC)
	RETURNS NUMERIC
	AS $$ declare
		LReturnVal NUMERIC(20,0);
	begin
		LReturnVal := TRUNC(DAE_Frac(ATimeSpan / (600000000 * 60)) * 60);
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_TSReadHour(ATimeSpan in NUMERIC)
	RETURNS NUMERIC
	AS $$ declare
		LReturnVal NUMERIC(20,0);
	begin
		LReturnVal := TRUNC(DAE_Frac(ATimeSpan / (36000000000 * 24)) * 24);
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_TSReadDay(ATimeSpan in NUMERIC)
	RETURNS NUMERIC
	AS $$ declare
		LReturnVal NUMERIC(20,0);
	begin
		LReturnVal := TRUNC(ATimeSpan / 864000000000);
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_TSWriteMillisecond(ATimeSpan in NUMERIC, APart int)
	RETURNS NUMERIC
	AS $$ declare
		LReturnVal NUMERIC(20,0);
	begin
		LReturnVal := ATimeSpan + (APart - DAE_TSReadMillisecond(ATimeSpan)) * 10000;
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_TSWriteSecond(ATimeSpan in NUMERIC, APart int)
	RETURNS NUMERIC
	AS $$ declare
		LReturnVal NUMERIC(20,0);
	begin
		LReturnVal := ATimeSpan + (APart - DAE_TSReadMillisecond(ATimeSpan)) * 10000000;
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_TSWriteMinute(ATimeSpan in NUMERIC, APart int)
	RETURNS NUMERIC
	AS $$ declare
		LReturnVal NUMERIC(20,0);
	begin
		LReturnVal := ATimeSpan + (APart - DAE_TSReadMillisecond(ATimeSpan)) * 600000000;
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_TSWriteHour(ATimeSpan in NUMERIC, APart int)
	RETURNS NUMERIC
	AS $$ declare
		LReturnVal NUMERIC(20,0);
	begin
		LReturnVal := ATimeSpan + (APart - DAE_TSReadMillisecond(ATimeSpan)) * 36000000000;
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_TSWriteDay(ATimeSpan in NUMERIC, APart int)
	RETURNS NUMERIC
	AS $$ declare
		LReturnVal NUMERIC(20,0);
	begin
		LReturnVal := ATimeSpan + (APart - DAE_TSReadMillisecond(ATimeSpan)) * 864000000000;
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_AddYears(ADateTime in date, AYears in int)
	RETURNS date
	AS $$ declare
		LReturnVal date;
	begin
		LReturnVal := ADD_MONTHS(ADateTime, AYears * 12);
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_Today()
	RETURNS date
	AS $$ declare
		LReturnVal date;
	begin
		LReturnVal := TRUNC(SysDate);
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DaysInMonth(AYear in integer, AMonth in int)
	RETURNS integer
	AS $$ declare
		LReturnVal int;
	begin
		/*LDate := To_Date(1 + AMonth * 100 + AYear * 10000,'YYYY MM DD');cannot create variables*/
		LReturnVal := Last_Day(To_Date(1 + AMonth * 100 + AYear * 10000,'YYYY MM DD')) - To_Date(1 + AMonth * 100 + AYear * 10000,'YYYY MM DD') + 1;
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_IsLeapYear(AYear in int)
	RETURNS int
	AS $$ declare 
		LReturnVal int;
	begin
		LReturnVal := To_Date('01/03/' || To_Char(AYear),'dd/mm/yyyy') - To_Date('28/02/' || To_Char(AYear),'dd/mm/yyyy') - 1;
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DTReadDay(ADateTime in date)
	RETURNS int
	AS $$ declare
		LReturnVal int;
	begin
		LReturnVal := To_Char(ADateTime, 'dd');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DTReadMonth(ADateTime in date)
	RETURNS int
	AS $$ declare
		LReturnVal int;
	begin
		LReturnVal := To_Char(ADateTime, 'mm');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DTReadYear(ADateTime in date)
	RETURNS int
	AS $$ declare
		LReturnVal int;
	begin
		LReturnVal := To_Char(ADateTime, 'yyyy');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DTReadMinute(ADateTime in date)
	RETURNS int
	AS $$ declare
		LReturnVal int;
	begin
		LReturnVal := To_Char(ADateTime, 'mi');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DTReadSecond(ADateTime in date)
	RETURNS int
	AS $$ declare
		LReturnVal int;
	begin
		LReturnVal := To_Char(ADateTime, 'ss');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DTReadMillisecond(ADateTime in date)
	RETURNS int
	AS $$ declare
		LReturnVal int;
	begin
		LReturnVal := 0;
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DayOfYear(ADateTime in date)
	RETURNS int
	AS $$ declare 
		LReturnVal int;
	begin
		LReturnVal := To_Char(ADateTime, 'ddd');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DayOfWeek(ADateTime in date)
	RETURNS int
	AS $$ declare 
		LReturnVal int;
	begin
		LReturnVal := To_Char(ADateTime, 'd');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DTWriteMillisecond(ADateTime in date, APart in int)
	RETURNS date
	AS $$ declare
		LReturnVal Date;
	begin
		LReturnVal := ADateTime;
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DTWriteSecond(ADateTime in date, APart in int)
	RETURNS date
	AS $$ declare 
		LReturnVal Date;
	begin
		LReturnVal := To_Date(To_Char(ADateTime,'yyyy') || '/' || To_Char(ADateTime,'mm') || '/' || To_Char(ADateTime,'dd') || ' ' || To_Char(ADateTime,'hh24') || ':' || To_Char(ADateTime,'mi') || ':' || to_Char(APart), 'yyyy/mm/dd hh24:mi:ss');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DTWriteMinute(ADateTime in date, APart in int)
	RETURNS date
	AS $$ declare 
		LReturnVal Date;
	begin
		LReturnVal := To_Date(To_Char(ADateTime,'yyyy') || '/' || To_Char(ADateTime,'mm') || '/' || To_Char(ADateTime,'dd') || ' ' || To_Char(ADateTime,'hh24') || ':' || To_Char(APart) || ':' || to_Char(ADateTime,'ss'), 'yyyy/mm/dd hh24:mi:ss');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DTWriteHour(ADateTime in date, APart in int)
	RETURNS date
	AS $$ declare 
		LReturnVal Date;
	begin
		LReturnVal := To_Date(To_Char(ADateTime,'yyyy') || '/' || To_Char(ADateTime,'mm') || '/' || To_Char(ADateTime,'dd') || ' ' || To_Char(APart) || ':' || To_Char(ADateTime,'mi') || ':' || to_Char(ADateTime,'ss'), 'yyyy/mm/dd hh24:mi:ss');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DTWriteDay(ADateTime in date, APart in int)
	RETURNS date
	AS $$ declare 
		LReturnVal Date;
	begin
		LReturnVal := To_Date(To_Char(ADateTime,'yyyy') || '/' || To_Char(ADateTime,'mm') || '/' || To_Char(APart) || ' ' || To_Char(ADateTime,'hh24') || ':' || To_Char(ADateTime,'mi') || ':' || to_Char(ADateTime,'ss'), 'yyyy/mm/dd hh24:mi:ss');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DTWriteMonth(ADateTime in date, APart in int)
	RETURNS date
	AS $$ declare 
		LReturnVal Date;
	begin
		LReturnVal := To_Date(To_Char(ADateTime,'yyyy') || '/' || To_Char(APart) || '/' || To_Char(ADateTime,'dd') || ' ' || To_Char(ADateTime,'hh24') || ':' || To_Char(ADateTime,'mi') || ':' || to_Char(ADateTime,'ss'), 'yyyy/mm/dd hh24:mi:ss');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DTWriteYear(ADateTime in date, APart in int)
	RETURNS date
	AS $$ declare 
		LReturnVal Date;
	begin
		LReturnVal := To_Date(To_Char(APart) || '/' || To_Char(ADateTime,'mm') || '/' || To_Char(ADateTime,'dd') || ' ' || To_Char(ADateTime,'hh24') || ':' || To_Char(ADateTime,'mi') || ':' || to_Char(ADateTime,'ss'), 'yyyy/mm/dd hh24:mi:ss');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_TSDateTime(ATimeSpan in NUMERIC)
	RETURNS date
	AS $$ declare 
		LReturnVal Date;
	begin
		LReturnVal := round((ATimeSpan - 630822816000000000)/864000000000,0) + To_Date(20000101 * 100000 + round(mod(ATimeSpan/10000000 , 86400),0), 'yyyy dd mm sssss');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DTTimeSpan(ADateTime in date)
	RETURNS NUMERIC
	AS $$ declare 
		LReturnVal NUMERIC;
	begin
		LReturnVal := 631139040000000000 + ((ADateTime - To_Date('01-JAN-2001')) * 86400 + to_char(ADateTime,'sssss')) * 10000000;
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DateTimeSelector1(AYear in int)
	RETURNS date
	AS $$ declare 
		LReturnVal date;
	begin
		LReturnVal := to_Date(AYear * 10000 + 0101, 'yyyy mm dd');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DateTimeSelector2(AYear in int, AMonth in int)
	RETURNS date
	as  $$ declare
		LReturnVal date;
	begin
		LReturnVal := to_Date(AYear * 10000 + AMonth * 100 + 01, 'yyyy mm dd');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DateTimeSelector3(AYear in int, AMonth in int, ADay in int)
	RETURNS date
	AS $$ declare 
		LReturnVal date;
	begin
		LReturnVal := to_Date(AYear * 10000 + AMonth * 100 + ADay, 'yyyy mm dd');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DateTimeSelector4(AYear in int, AMonth in int, ADay in int, AHour in int)
	RETURNS date
	AS $$ declare 
		LReturnVal date;
	begin
		LReturnVal := to_Date((AYear * 1000000 + AMonth * 10000 + ADay * 100 + AHour), 'yyyy mm dd hh24');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DateTimeSelector5(AYear in int, AMonth in int, ADay in int, AHour in int, AMinute in int)
	RETURNS date
	AS $$ declare 
		LReturnVal date;
	begin
		LReturnVal := to_Date((AYear * 100000000 + AMonth * 1000000 + ADay * 10000 + AHour * 100 + AMinute), 'yyyy mm dd hh24 mi');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DateTimeSelector6(AYear in int, AMonth in int, ADay in int, AHour in int, AMinute in int, ASecond in int)
	RETURNS date
	AS $$ declare 
		LReturnVal date;
	begin
		LReturnVal := to_Date((AYear * 10000000000 + AMonth * 100000000 + ADay * 1000000 + AHour * 10000 + AMinute * 100 + ASecond), 'yyyy mm dd hh24 mi ss');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;
\
create or replace function DAE_DateTimeSelector7(AYear in int, AMonth in int, ADay in int, AHour in int, AMinute in int, ASecond in int, AMillisecond in int)
	RETURNS date
	AS $$ declare 
		LReturnVal date;
	begin
		LReturnVal := to_Date((AYear * 10000000000 + AMonth * 100000000 + ADay * 1000000 + AHour * 10000 + AMinute * 100 + ASecond), 'yyyy mm dd hh24 mi ss');
		RETURN LReturnVal;
	end;
	$$ LANGUAGE plpgsql;