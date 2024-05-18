# -*- coding: utf-8 -*-
"""
Created on Sun Feb 18 22:39:00 2024

@author: wolflou1, zwahlni2
"""
import numpy as np
import matplotlib.pyplot as plt
import csv

# Time Intervalls for Graphs: -------------------------------------------------

t_from_normal = 6
t_to_normal = 18

t_from_zoom = 12.3
t_to_zoom = 13.5

# -----------------------------------------------------------------------------

t1 = []
x1_t = []
v1_t = []
a1_t = []
p1_t = []
eKin1_t = []
fFeder_t = []

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
        a1_t.append(float(row[6]))
        fFeder_t.append((float(row[7])))

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

plt.figure(0)
plt.plot(t1, fFeder_t, color='r', linestyle='-', label= 'moving Cube')
plt.xlabel("Time [s]")
plt.ylabel("Spring Force [N]")
plt.title("Spring Force over Time in x-Axis")
plt.xlim(0, 9.4)
plt.ylim(-140, 140)
plt.yticks(range(-140, 141, 20))
plt.legend()
plt.grid()


plt.figure(1)
plt.plot(t1, a1_t, color='r', linestyle='-', label= 'moving Cube')
plt.xlabel("Time [s]")
plt.ylabel(r"Acceleration [m/s$^2$]")
plt.title("Acceleration over Time in x-Axis")
plt.xlim(0, 9.4)
plt.ylim(-14, 14)
plt.yticks(range(-14, 15, 2))
plt.legend()
plt.grid()

plt.figure(2)
plt.plot(t1, x1_t, color='r', linestyle='-', label= 'moving Cube')
plt.plot(t2, x2_t, color='b', linestyle='-', label= 'stationary Cube')
plt.xlabel("Time [s]")
plt.ylabel("Position [m]")
plt.title("Position over Time in x-Axis")
plt.xlim(t_from_normal, t_to_normal)
plt.ylim(-5, 25)
plt.legend()
plt.grid()

plt.figure(3)
plt.plot(t1, v1_t, color='r', linestyle='-', label= 'moving Cube')
plt.plot(t2, v2_t, color='b', linestyle='-', label= 'stationary Cube')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Velocity [m/s]")
plt.title("Velocity over Time")
plt.xlim(t_from_normal, t_to_normal)
plt.ylim(-7, 7)
plt.legend()
plt.grid()

totalImpulse = np.array(p1_t) + np.array(p2_t)

plt.figure(4)
plt.plot(t1, totalImpulse, color='black', linestyle='-', label= 'total')
plt.plot(t1, p1_t, color='r', linestyle='-', label= 'moving Cube')
plt.plot(t2, p2_t, color='b', linestyle='-', label= 'stationary Cube')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Impulse [kg*m/s]")
plt.title("Impulse over Time")
plt.xlim(t_from_normal, t_to_normal)
plt.ylim(-80, 80)
plt.legend()
plt.grid()

totalEnergy = np.array(eKin1_t) + np.array(eKin2_t) + np.array(eSpr_t)

plt.figure(5)
plt.plot(t1, totalEnergy, color='black', linestyle='-', label= 'total')
plt.plot(t1, eKin1_t, color='r', linestyle='-', label= 'moving Cube')
plt.plot(t2, eKin2_t, color='b', linestyle='-', label= 'stationary Cube')
plt.plot(t1, eSpr_t, color='g', linestyle='-', label= 'Spring')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Energy [J]")
plt.title("Energy over Time")
plt.xlim(t_from_normal, t_to_normal)
plt.ylim(-3, 200)
plt.legend()
plt.grid()

plt.figure(6)
plt.plot(t1, totalEnergy, color='black', linestyle='-', label= 'total')
plt.plot(t1, eKin1_t, color='r', linestyle='-', label= 'moving Cube')
plt.plot(t2, eKin2_t, color='b', linestyle='-', label= 'stationary Cube')
plt.plot(t1, eSpr_t, color='g', linestyle='-', label= 'Spring')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Energy [J]")
plt.title("Energy over Time (Zoomed in)")
plt.xlim(t_from_zoom, t_to_zoom)
plt.ylim(-3, 150)
plt.legend()
plt.grid()