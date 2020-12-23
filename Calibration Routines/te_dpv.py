# TE-DPV calibration
import pandas as pd
import genetic_algorithm as ga
import os

while True:
    while True:
        file = input("Data set to import: ")
        try:
            data_set = pd.read_csv(file)
            break
        except Exception:
            try:
                data_set = pd.read_csv(file + ".csv")
                break
            except Exception:
                print("No file found.")
    print(data_set.Viscosity.nunique(dropna=True))
    if (len(data_set.index) < 3):
        print("At least three samples runs are required.")
    elif (data_set.Viscosity.nunique(dropna=True) < 2):
        print("Measurements of more than one viscosity are required.")
    else:
        break
os.system('cls')
data_set.insert(0, 'Calibrate', True)
data_set['Torque_binary'] = [int(j, 16) for j in list(data_set.Torque)]
data_set['Calculated Viscosity'] = 0
data_set['% Error'] = 0
data_set['Include'] = True
i = list(data_set.index.values)
x = data_set.Viscosity
y = data_set.Torque_binary
z = 0


def myfunc(x):
    return slope*x+intercept


while True:
    working_set = ga.build_ws(data_set)

    if (len(working_set.index) > 2):
        slope, intercept, r, p, std_err, calc_visc, reading_error, tss,\
            avg_err = ga.calculations(data_set, working_set)
        data_set['Calculated Viscosity'] = calc_visc
        data_set['% Error'] = reading_error
        print(data_set)
        print()
        print("y = {} x + {}".format(slope, intercept))
        print("r2 = {}".format(r*r))
        print("Standard Error (working data set) = {}".format(std_err))
        print("Total sum of the squares = {}".format(tss))
        print("Average error (complete data set) = {} ".format(avg_err))

    else:
        print("Unable to calculate. At least three samples are required.\n")
        data_set['Calculated Viscosity'] = abs(calc_visc * 0)
        data_set['% Error'] = abs(reading_error * 0)
        print(data_set)
        print()

    c = input("""
    Index number to toggle.
    A/a to run automatic selection (Calibrate & Include reset).
    B/b run automatic selection (no reset).
    C/c Select by viscosity constraints (no reset).
    I/i Select by index constraints (no reset).
    M/m print merged plot.
    P/p print new plot.
    R/r to reset.
    Q/q to quit.
    """)
    if c == "":
        os.system('cls')
    elif c in ('A', 'a'):
        data_set["Calibrate"] = True
        data_set['Include'] = True
        working_set = ga.build_ws(data_set)
        slope, intercept, r, p, std_err, calc_visc, reading_error, tss,\
            avg_err = ga.calculations(data_set, working_set)
        ga.genetic_algorithm(data_set, tss)
        os.system('cls')
    elif c in ('B', 'b'):
        working_set = ga.build_ws(data_set)
        slope, intercept, r, p, std_err, calc_visc, reading_error, tss,\
            avg_err = ga.calculations(data_set, working_set)
        ga.genetic_algorithm(data_set, tss)
        os.system('cls')
    elif c in ('C', 'c'):
        ll = input("Lower Viscosity Limit: ")
        if ll == '':
            ll = 0
        ul = input("Upper Viscosity limit: ")
        if ul == '':
            ul = 5000
        data_set = ga.constrain(data_set, int(ll), int(ul))
        os.system('cls')
    elif c in ('I', 'i'):
        ll = input("Lower Index Limit: ")
        if ll == '':
            ll = 0
        ul = input("Upper Index limit: ")
        if ul == '':
            ul = len(data_set.index) - 1
        data_set = ga.iconstrain(data_set, int(ll), int(ul))
        os.system('cls')
    elif c in ('M', 'm'):
        if z == 0:
            z = 1
        ga.display_plot(x, y, myfunc, z, slope, intercept, r)
        os.system('cls')
    elif c in ('P', 'p'):
        z = z + 1
        ga.display_plot(x, y, myfunc, z, slope, intercept, r)
#        os.system('cls')
    elif c in ('R', 'r'):
        while True:
            b = input("""
            (1) Reset Calibrate Flag
            (2) Reset Include Flag
            (3) Reset Both
            """)
            if b == "":
                b = 3
            if int(b) == 1:
                data_set['Calibrate'] = True
                break
            elif int(b) == 2:
                data_set['Include'] = True
                break
            else:
                data_set['Calibrate'] = True
                data_set['Include'] = True
                break
        os.system('cls')
    elif c in ('Q', 'q'):
        break
    elif c in str(i):
        while True:
            b = input("""
            (1) Toggle Calibrate Flag
            (2) Toggle Include Flag
            (3) Toggle Both
            """)
            if int(b) == 1:
                data_set.at[int(c), 'Calibrate'] ^= True
                break
            elif int(b) == 2:
                data_set.at[int(c), 'Include'] ^= True
                break
            elif int(b) == 3:
                data_set.at[int(c), 'Calibrate'] ^= True
                data_set.at[int(c), 'Include'] ^= True
                break
        os.system('cls')
    else:
        os.system('cls')
