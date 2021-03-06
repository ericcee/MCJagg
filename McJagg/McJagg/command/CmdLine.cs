﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using McJagg.map;

namespace McJagg.command
{
    class CmdLine : Command
    {
        public override string getName()
        {
            return "line";
        }

        public override string getHelp()
        {
            throw new NotImplementedException();
        }

        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{},
            };
        }

        public override int getDefaultPermissionLevel()
        {
            return 100;
        }

        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            Block b1 = getBlockInput(executor, "Please select the first Block");
            Block b2 = getBlockInput(executor, "Please select the Second Block");
            DrawLineTo(b1.getX(), b1.getY(), b1.getZ(), b2.getX(), b2.getY(), b2.getZ(), 1, ((Player)executor).getMap());
        }

        private void DrawLineTo(int x1, int y1, int z1,  int x2,  int y2,  int z2, int symbol,map.Map thMap){
            int i, dx, dy, dz, l, m, n, x_inc, y_inc, z_inc, err_1, err_2, dx2, dy2, dz2;
            int[] point= new int[3];
    
            point[0] = x1;
            point[1] = y1;
            point[2] = z1;
            dx = x2 - x1;
            dy = y2 - y1;
            dz = z2 - z1;
            x_inc = (dx < 0) ? -1 : 1;
            l = Math.Abs(dx);
            y_inc = (dy < 0) ? -1 : 1;
            m = Math.Abs(dy);
            z_inc = (dz < 0) ? -1 : 1;
            n = Math.Abs(dz);
            dx2 = l << 1;
            dy2 = m << 1;
            dz2 = n << 1;
    
            if ((l >= m) && (l >= n)) {
                err_1 = dy2 - l;
                err_2 = dz2 - l;
                for (i = 0; i < l; i++) {
                    thMap.setBlockContent(point[0], point[1], point[2], symbol, true);
                    if (err_1 > 0) {
                        point[1] += y_inc;
                        err_1 -= dx2;
                    }
                    if (err_2 > 0) {
                        point[2] += z_inc;
                        err_2 -= dx2;
                    }
                    err_1 += dy2;
                    err_2 += dz2;
                    point[0] += x_inc;
                }
            } else if ((m >= l) && (m >= n)) {
                err_1 = dx2 - m;
                err_2 = dz2 - m;
                for (i = 0; i < m; i++)
                {
                    thMap.setBlockContent(point[0], point[1], point[2], symbol, true);
                    if (err_1 > 0) {
                        point[0] += x_inc;
                        err_1 -= dy2;
                    }
                    if (err_2 > 0) {
                        point[2] += z_inc;
                        err_2 -= dy2;
                    }
                    err_1 += dx2;
                    err_2 += dz2;
                    point[1] += y_inc;
                }
            } else {
                err_1 = dy2 - n;
                err_2 = dx2 - n;
                for (i = 0; i < n; i++)
                {
                    thMap.setBlockContent(point[0], point[1], point[2], symbol, true);
                    if (err_1 > 0) {
                        point[1] += y_inc;
                        err_1 -= dz2;
                    }
                    if (err_2 > 0) {
                        point[0] += x_inc;
                        err_2 -= dz2;
                    }
                    err_1 += dy2;
                    err_2 += dx2;
                    point[2] += z_inc;
                }
            } 
            thMap.setBlockContent(point[0], point[1], point[2], symbol, true);
        }
    }
}
