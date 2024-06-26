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

t_from_rot = 13
t_to_rot = 40

# -----------------------------------------------------------------------------

t1 = []
x1_t = []
v1_t = []
p1_t = []
eKin1_t = []

t2 = []
x2_t = []
z2_t = []
v2_t = []
p2_t = []
eKin2_t = []
# Grössen zur Rotation des L Förmigen Körpers mit blauem Würfel attached:
alpha_t = []
vRot_t = []
pIntrinsicRot_t = []
pOrbitalRot_t = []
pRotTot_t = []
eRot_t = []
# Grössen zur Translation des Schwerpunkts von L Förmigen Körper + blauer Würfel
xTrans_t = []
zTrans_t = []
vTrans_t = []
pTrans_t = []
eTrans_t = []

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
        z2_t.append(float(row[2]))
        v2_t.append(float(row[3]))
        p2_t.append(float(row[4]))
        eKin2_t.append(float(row[5]))
        alpha_t.append(float(row[6]))
        vRot_t.append(float(row[7]))
        pIntrinsicRot_t.append(float(row[8]))
        pOrbitalRot_t.append(float(row[9]))
        pRotTot_t.append(float(row[10]))
        eRot_t.append(float(row[11]))
        xTrans_t.append(float(row[12]))
        zTrans_t.append(float(row[13]))
        vTrans_t.append(float(row[14]))
        pTrans_t.append(float(row[15]))
        eTrans_t.append(float(row[16]))

plt.figure(1)
plt.plot(t1, x1_t, color='r', linestyle='-', label= 'red Cube')
plt.plot(t2, x2_t, color='b', linestyle='-', label= 'blue Cube')
plt.xlabel("Time [s]")
plt.ylabel("Position [m]")
plt.title("Position over Time")
plt.xlim(t_from_normal, t_to_normal)
plt.ylim(-5, 25)
plt.legend()
plt.grid()

plt.figure(2)
plt.plot(t1, v1_t, color='r', linestyle='-', label= 'red Cube')
plt.plot(t2, v2_t, color='b', linestyle='-', label= 'blue Cube')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Velocity [m/s]")
plt.title("Velocity over Time")
plt.xlim(t_from_normal, t_to_normal)
plt.ylim(-7, 7)
plt.legend()
plt.grid()

totalImpulse = np.array(p1_t) + np.array(p2_t)

plt.figure(3)
plt.plot(t1, totalImpulse, color='black', linestyle='-', label= 'total')
plt.plot(t1, p1_t, color='r', linestyle='-', label= 'red Cube')
plt.plot(t2, p2_t, color='b', linestyle='-', label= 'blue Cube')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Impulse [kg*m/s]")
plt.title("Impulse over Time")
plt.xlim(t_from_normal, t_to_normal)
plt.ylim(-80, 80)
plt.legend()
plt.grid()

totalEnergy = np.array(eKin1_t) + np.array(eKin2_t) + np.array(eSpr_t)

plt.figure(4)
plt.plot(t1, totalEnergy, color='black', linestyle='-', label= 'total')
plt.plot(t1, eKin1_t, color='r', linestyle='-', label= 'red Cube')
plt.plot(t2, eKin2_t, color='b', linestyle='-', label= 'blue Cube')
plt.plot(t1, eSpr_t, color='g', linestyle='-', label= 'Spring')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Energy [J]")
plt.title("Energy over Time")
plt.xlim(t_from_normal, t_to_normal)
plt.ylim(-3, 200)
plt.legend()
plt.grid()

plt.figure(5)
plt.plot(t1, totalEnergy, color='black', linestyle='-', label= 'total')
plt.plot(t1, eKin1_t, color='r', linestyle='-', label= 'red Cube')
plt.plot(t2, eKin2_t, color='b', linestyle='-', label= 'blue Cube')
plt.plot(t1, eSpr_t, color='g', linestyle='-', label= 'Spring')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Energy [J]")
plt.title("Energy over Time (Zoomed in)")
plt.xlim(t_from_zoom, t_to_zoom)
plt.ylim(-3, 150)
plt.legend()
plt.grid()

# Teil 3:

plt.figure(6)
plt.plot(t2, z2_t, color='b', linestyle='-', label= 'blue Cube')
plt.plot(t2, zTrans_t, color='y', linestyle='-', label= 'combined body')
plt.xlabel("Time [s]")
plt.ylabel("Position [m]")
plt.title("z-Position over Time")
plt.xlim(t_from_rot, t_to_rot)
plt.ylim(-1.5, 2.5)
plt.legend()
plt.grid()

plt.figure(7)
plt.plot(t2, v2_t, color='b', linestyle='-', label= 'blue Cube')
plt.plot(t2, vTrans_t, color='y', linestyle='-', label= 'combined body')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Velocity [m/s]")
plt.title("Velocity over Time")
plt.xlim(t_from_rot, t_to_rot)
plt.ylim(-1, 5)
plt.legend()
plt.grid()

plt.figure(10)
plt.plot(t2, alpha_t, color='y', linestyle='-')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Angle [rad]")
plt.title("Rotation angle of combined body over Time")
plt.xlim(t_from_rot, t_to_rot)
plt.ylim(-2, 8)
plt.grid()

plt.figure(11)
plt.plot(t2, vRot_t, color='y', linestyle='-')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("rotation speed [rad/s]")
plt.title("Rotation speed over Time")
plt.xlim(t_from_rot, t_to_rot)
plt.ylim(-0.5, 2)
plt.grid()

plt.figure(8)
plt.plot(t2, pRotTot_t, color='black', linestyle='-', label= 'Total')
plt.plot(t2, pIntrinsicRot_t, color='orange', linestyle='-', label= 'Intrinsic')
plt.plot(t2, pOrbitalRot_t, color='green', linestyle='-', label= 'Orbital')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Anular momentum [kg*m^2/s]")
plt.title("Angular momentum of the combined body over Time")
plt.xlim(t_from_rot, t_to_rot)
plt.ylim(-75, 75)
plt.legend()
plt.grid()

eTransUndRot = np.array(eTrans_t) + np.array(eRot_t)

plt.figure(9)
plt.plot(t2, eTransUndRot, color='black', linestyle='-', label= 'E_trans+rot')
plt.plot(t2, eKin2_t, color='b', linestyle='-', label= 'E_trans blue cube')
plt.plot(t2, eTrans_t, color='y', linestyle='-', label= 'E_trans combined body')
plt.plot(t2, eRot_t, color='g', linestyle='-', label= 'E_rot_ combined body')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Energy [J]")
plt.title("Energy over Time")
plt.xlim(t_from_rot, t_to_rot)
plt.ylim(-3, 200)
plt.legend()
plt.grid()