# -*- coding: utf-8 -*-
"""
Created on Sun Feb 18 22:39:00 2024

@author: wolflou1, zwahlni2
"""
import numpy as np
import matplotlib.pyplot as plt
import csv

t1 = []
x1_t = []
v1_t = []
p1_t = []
eKin1_t = []

t2 = []
x2_t = []
v2_t = []
p2_t = []
eKin2_t = []

eSpr_t = []

with open('movingCube_stats.csv', 'r') as csvfile1:
    # Überspringe die Header-Zeile
    next(csvfile1)
    plots = csv.reader(csvfile1, delimiter = ',')
    
    for row in plots:
        t1.append(float(row[0]))
        x1_t.append(float(row[1]))
        v1_t.append(float(row[2]))
        p1_t.append(float(row[3]))
        eKin1_t.append(float(row[4]))
        eSpr_t.append(float(row[5]))

with open('stationaryCube_stats.csv', 'r') as csvfile2:
    # Überspringe die Header-Zeile
    next(csvfile2)
    plots = csv.reader(csvfile2, delimiter = ',')
    
    for row in plots:
        t2.append(float(row[0]))
        x2_t.append(float(row[1]))
        v2_t.append(float(row[2]))
        p2_t.append(float(row[3]))
        eKin2_t.append(float(row[4]))

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

totalImpulse = np.array(p1_t) + np.array(p2_t)

plt.figure(3)
plt.plot(t1, totalImpulse, color='black', linestyle='-', label= 'total')
plt.plot(t1, p1_t, color='r', linestyle='-', label= 'moving Cube')
plt.plot(t2, p2_t, color='b', linestyle='-', label= 'stationary Cube')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Impulse [kg*m/s]")
plt.title("Impulse over Time")
plt.xlim(0, 6)
plt.ylim(-20, 80)
plt.legend()
plt.grid()

totalEnergy = np.array(eKin1_t) + np.array(eKin2_t) + np.array(eSpr_t)

plt.figure(4)
plt.plot(t1, totalEnergy, color='black', linestyle='-', label= 'total')
plt.plot(t1, eKin1_t, color='r', linestyle='-', label= 'moving Cube')
plt.plot(t2, eKin2_t, color='b', linestyle='-', label= 'stationary Cube')
plt.plot(t1, eSpr_t, color='g', linestyle='-', label= 'Spring')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Energy [J]")
plt.title("Energy over Time")
plt.xlim(0, 6)
plt.ylim(-3, 150)
plt.legend()
plt.grid()

plt.figure(5)
plt.plot(t1, totalEnergy, color='black', linestyle='-', label= 'total')
plt.plot(t1, eKin1_t, color='r', linestyle='-', label= 'moving Cube')
plt.plot(t2, eKin2_t, color='b', linestyle='-', label= 'stationary Cube')
plt.plot(t1, eSpr_t, color='g', linestyle='-', label= 'Spring')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Energy [J]")
plt.title("Energy over Time (Zoomed in)")
plt.xlim(2, 4)
plt.ylim(-3, 150)
plt.legend()
plt.grid()