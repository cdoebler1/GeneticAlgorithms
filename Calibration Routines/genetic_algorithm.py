from scipy import stats
import math
import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
import statsmodels.formula.api as smf
import statsmodels.api as sm


def build_ws(data_set):
    working_set = data_set.loc[data_set['Calibrate']].copy()
    working_set = working_set.loc[:, ['Viscosity', 'Torque_binary']]
    return working_set


def constrain(data_set, ll, ul):
    for k in data_set.index:
        if data_set.at[k, 'Viscosity'] < ll:
            data_set.at[int(k), 'Calibrate'] = False
            data_set.at[int(k), 'Include'] = False
        if data_set.at[k, 'Viscosity'] > ul:
            data_set.at[int(k), 'Calibrate'] = False
            data_set.at[int(k), 'Include'] = False
    return data_set


def iconstrain(data_set, ll, ul):
    for k in data_set.index:
        if k < ll:
            data_set.at[int(k), 'Calibrate'] = False
            data_set.at[int(k), 'Include'] = False
        if k > ul:
            data_set.at[int(k), 'Calibrate'] = False
            data_set.at[int(k), 'Include'] = False
    return data_set


def genetic_algorithm(data_set, old_tss):
    m = ''
    while True:
        working_set = build_ws(data_set)
        if (len(working_set.index) == 3):
            break
        for k in working_set.index:
            test_set = working_set.drop(k)
            slope, intercept, r, p, std_err, calc_visc, reading_error, tss,\
                avg_err = calculations(data_set, test_set)
            if tss < old_tss:
                old_tss = tss
                m = k
        if m:
            data_set.at[int(m), 'Calibrate'] ^= True
#            data_set.at[int(m), 'Include'] ^= True
            m = ''
        else:
            break
    data_set['Calculated Viscosity'] = calc_visc
    data_set['% Error'] = reading_error
    return data_set


def calculations(data_set, working_set):
    slope, intercept, r, p, std_err = stats.linregress(working_set)
    calc_visc = (data_set.Torque_binary-intercept)/slope
    measure_set = data_set.loc[data_set['Include']].copy()
    measure_set = measure_set.loc[:, ['Viscosity', 'Torque_binary']]
    calc_visc_sub = (measure_set.Torque_binary-intercept)/slope
    reading_error =\
        ((calc_visc_sub-measure_set.Viscosity)/measure_set.Viscosity)*100
    tss = sum(map(lambda j: j * j, reading_error))
    avg_err = math.sqrt(tss)/len(data_set.index)
    return slope, intercept, r, p, std_err, calc_visc, reading_error, tss,\
        avg_err


def display_plot(x, y, myfunc, z, slope, intercept, r):
    plt.ion()
    plt.figure(z)
    plt.title('TE-DPV Torque vs. Viscosity\n' + 'y = {} x + {}'.format(slope,
              intercept) + '\nr2 = {}'.format(r*r))
    plt.xlabel('Viscosity')
    plt.ylabel('Torque')
    plt.scatter(x, y)
    mymodel = list(map(myfunc, x))

    df = pd.DataFrame(columns=['y', 'x'])
    df['x'] = x
    df['y'] = y
    xx = np.linspace(0, 1000, 1000)
    poly_fit = np.poly1d(np.polyfit(x, y, 2))
    results = smf.ols(formula='y ~ poly_fit(x)', data=df).fit()
#    results = sm.OLS(y, x).fit()
    plt.plot(xx, poly_fit(xx), c='r', linestyle='-')
    plt.plot(x, mymodel)
    print(results.summary())
