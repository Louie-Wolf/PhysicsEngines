# -*- coding: utf-8 -*-
"""
Created on Sun Feb 18 22:39:00 2024

@author: wolflou1, zwahlni2
"""
import matplotlib.pyplot as plt
import csv

t = []
x_t = []
v_t = []
F_t = []

with open('time_series_aufg1.csv', 'r') as csvfile:
    # Überspringe die Header-Zeile
    next(csvfile)
    plots = csv.reader(csvfile, delimiter = ',')
    
    for row in plots:
        t.append(float(row[0]))
        x_t.append(float(row[1]))
        v_t.append(float(row[2]))
        F_t.append(float(row[3]))

plt.figure(1)
plt.plot(t, x_t, color='g', linestyle='-', label='Cube')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("x-Position [m]")
plt.title("x-Position over Time x(t)")
plt.xlim(min(t), max(t))
plt.ylim(min(x_t) - abs(min(x_t)/10), max(x_t) + max(x_t)/10)
plt.legend()
plt.grid()

plt.figure(2)
plt.plot(t, v_t, color='b', linestyle='-', label='Cube')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Velocity [m/s]")
plt.title("x-Velocity over Time v(t)")
plt.xlim(min(t), max(t))
plt.ylim(min(v_t) - abs(min(v_t)/10), max(v_t) + max(v_t)/10)
plt.legend()
plt.grid()

plt.figure(3)
plt.plot(t, F_t, color='r', linestyle='-', label='Cube')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Force [N]")
plt.title("x-Force over Time F(t)")
plt.xlim(min(t), max(t))
plt.ylim(min(F_t) - abs(min(F_t)/10), max(F_t) + max(F_t)/10)
plt.legend()
plt.grid()