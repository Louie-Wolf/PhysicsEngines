# -*- coding: utf-8 -*-
"""
Created on Sun Feb 18 22:39:00 2024

@author: wolflou1, zwahlni2
"""
import matplotlib.pyplot as plt
import csv

t1 = []
x1_t = []
v1_t = []

t2 = []
x2_t = []
v2_t = []

with open('movingCube_stats.csv', 'r') as csvfile1:
    # Überspringe die Header-Zeile
    next(csvfile1)
    plots = csv.reader(csvfile1, delimiter = ',')
    
    for row in plots:
        t1.append(float(row[0]))
        x1_t.append(float(row[1]))
        v1_t.append(float(row[2]))

with open('stationaryCube_stats.csv', 'r') as csvfile2:
    # Überspringe die Header-Zeile
    next(csvfile2)
    plots = csv.reader(csvfile2, delimiter = ',')
    
    for row in plots:
        t2.append(float(row[0]))
        x2_t.append(float(row[1]))
        v2_t.append(float(row[2]))

plt.figure(1)
plt.plot(t1, x1_t, color='r', linestyle='-', label= 'moving Cube')
plt.plot(t2, x2_t, color='b', linestyle='-', label= 'stationary Cube')
plt.xlabel("Time [s]")
plt.ylabel("Position [m]")
plt.title("Position over Time")
plt.xlim(0, 6)
plt.ylim(-15, 15)
plt.legend()
plt.grid()

plt.figure(2)
plt.plot(t1, v1_t, color='r', linestyle='-', label= 'moving Cube')
plt.plot(t2, v2_t, color='b', linestyle='-', label= 'stationary Cube')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Velocity [m/s]")
plt.title("Velocity over Time")
plt.xlim(0, 6)
plt.ylim(-3, 6)
plt.legend()
plt.grid()