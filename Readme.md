

## 不符合 SysFs 标准

按照 SysFs 标准，gpio 的逻辑文件夹名称应该为 gpio****，不应该为 P41_0 等等形式。

## gpio

```
# cat /sys/kernel/debug/gpio
gpiochip0: GPIOs 120-511, parent: platform/11030000.pin-controller, 11030000.pin-controller:
 gpio-120 (P0_0                )
 gpio-121 (P0_1                )
 gpio-122 (P0_2                )
 gpio-123 (P0_3                )
 gpio-124 (P0_4                )
 gpio-125 (P0_5                )
 gpio-126 (P0_6                )
 gpio-127 (P0_7                )
 gpio-128 (P1_0                )
 gpio-129 (P1_1                )
 gpio-130 (P1_2                )
 gpio-131 (P1_3                )
 gpio-132 (P1_4                )
 gpio-133 (P1_5                )
 gpio-134 (P1_6                )
 gpio-135 (P1_7                )
 gpio-136 (P2_0                )
 gpio-137 (P2_1                )
 gpio-138 (P2_2                )
 gpio-139 (P2_3                )
 gpio-140 (P2_4                )
 gpio-141 (P2_5                )
 gpio-142 (P2_6                )
 gpio-143 (P2_7                )
 gpio-144 (P3_0                )
 gpio-145 (P3_1                )
 gpio-146 (P3_2                )
 gpio-147 (P3_3                )
 gpio-148 (P3_4                )
 gpio-149 (P3_5                )
 gpio-150 (P3_6                )
 gpio-151 (P3_7                )
 gpio-152 (P4_0                )
 gpio-153 (P4_1                )
 gpio-154 (P4_2                )
 gpio-155 (P4_3                )
 gpio-156 (P4_4                )
 gpio-157 (P4_5                )
 gpio-158 (P4_6                )
 gpio-159 (P4_7                )
 gpio-160 (P5_0                )
 gpio-161 (P5_1                )
 gpio-162 (P5_2                )
 gpio-163 (P5_3                )
 gpio-164 (P5_4                )
 gpio-165 (P5_5                )
 gpio-166 (P5_6                )
 gpio-167 (P5_7                )
 gpio-168 (P6_0                )
 gpio-169 (P6_1                )
 gpio-170 (P6_2                )
 gpio-171 (P6_3                )
 gpio-172 (P6_4                )
 gpio-173 (P6_5                )
 gpio-174 (P6_6                )
 gpio-175 (P6_7                )
 gpio-176 (P7_0                )
 gpio-177 (P7_1                )
 gpio-178 (P7_2                )
 gpio-179 (P7_3                )
 gpio-180 (P7_4                )
 gpio-181 (P7_5                )
 gpio-182 (P7_6                )
 gpio-183 (P7_7                )
 gpio-184 (P8_0                )
 gpio-185 (P8_1                )
 gpio-186 (P8_2                )
 gpio-187 (P8_3                )
 gpio-188 (P8_4                )
 gpio-189 (P8_5                )
 gpio-190 (P8_6                )
 gpio-191 (P8_7                )
 gpio-192 (P9_0                )
 gpio-193 (P9_1                )
 gpio-194 (P9_2                )
 gpio-195 (P9_3                )
 gpio-196 (P9_4                )
 gpio-197 (P9_5                )
 gpio-198 (P9_6                )
 gpio-199 (P9_7                )
 gpio-200 (P10_0               )
 gpio-201 (P10_1               )
 gpio-202 (P10_2               )
 gpio-203 (P10_3               )
 gpio-204 (P10_4               )
 gpio-205 (P10_5               )
 gpio-206 (P10_6               )
 gpio-207 (P10_7               )
 gpio-208 (P11_0               )
 gpio-209 (P11_1               )
 gpio-210 (P11_2               )
 gpio-211 (P11_3               )
 gpio-212 (P11_4               )
 gpio-213 (P11_5               )
 gpio-214 (P11_6               )
 gpio-215 (P11_7               )
 gpio-216 (P12_0               )
 gpio-217 (P12_1               )
 gpio-218 (P12_2               )
 gpio-219 (P12_3               )
 gpio-220 (P12_4               )
 gpio-221 (P12_5               )
 gpio-222 (P12_6               )
 gpio-223 (P12_7               )
 gpio-224 (P13_0               )
 gpio-225 (P13_1               )
 gpio-226 (P13_2               )
 gpio-227 (P13_3               )
 gpio-228 (P13_4               )
 gpio-229 (P13_5               )
 gpio-230 (P13_6               )
 gpio-231 (P13_7               )
 gpio-232 (P14_0               )
 gpio-233 (P14_1               )
 gpio-234 (P14_2               )
 gpio-235 (P14_3               )
 gpio-236 (P14_4               )
 gpio-237 (P14_5               )
 gpio-238 (P14_6               )
 gpio-239 (P14_7               )
 gpio-240 (P15_0               )
 gpio-241 (P15_1               )
 gpio-242 (P15_2               )
 gpio-243 (P15_3               )
 gpio-244 (P15_4               )
 gpio-245 (P15_5               )
 gpio-246 (P15_6               )
 gpio-247 (P15_7               )
 gpio-248 (P16_0               )
 gpio-249 (P16_1               )
 gpio-250 (P16_2               )
 gpio-251 (P16_3               )
 gpio-252 (P16_4               )
 gpio-253 (P16_5               )
 gpio-254 (P16_6               )
 gpio-255 (P16_7               )
 gpio-256 (P17_0               )
 gpio-257 (P17_1               )
 gpio-258 (P17_2               )
 gpio-259 (P17_3               )
 gpio-260 (P17_4               )
 gpio-261 (P17_5               )
 gpio-262 (P17_6               )
 gpio-263 (P17_7               )
 gpio-264 (P18_0               )
 gpio-265 (P18_1               )
 gpio-266 (P18_2               )
 gpio-267 (P18_3               )
 gpio-268 (P18_4               )
 gpio-269 (P18_5               )
 gpio-270 (P18_6               )
 gpio-271 (P18_7               )
 gpio-272 (P19_0               |cd                  ) in  hi ACTIVE LOW
 gpio-273 (P19_1               )
 gpio-274 (P19_2               )
 gpio-275 (P19_3               )
 gpio-276 (P19_4               )
 gpio-277 (P19_5               )
 gpio-278 (P19_6               )
 gpio-279 (P19_7               )
 gpio-280 (P20_0               )
 gpio-281 (P20_1               )
 gpio-282 (P20_2               )
 gpio-283 (P20_3               )
 gpio-284 (P20_4               )
 gpio-285 (P20_5               )
 gpio-286 (P20_6               )
 gpio-287 (P20_7               )
 gpio-288 (P21_0               )
 gpio-289 (P21_1               )
 gpio-290 (P21_2               )
 gpio-291 (P21_3               )
 gpio-292 (P21_4               )
 gpio-293 (P21_5               )
 gpio-294 (P21_6               )
 gpio-295 (P21_7               )
 gpio-296 (P22_0               )
 gpio-297 (P22_1               )
 gpio-298 (P22_2               )
 gpio-299 (P22_3               )
 gpio-300 (P22_4               )
 gpio-301 (P22_5               )
 gpio-302 (P22_6               )
 gpio-303 (P22_7               )
 gpio-304 (P23_0               )
 gpio-305 (P23_1               )
 gpio-306 (P23_2               )
 gpio-307 (P23_3               )
 gpio-308 (P23_4               )
 gpio-309 (P23_5               )
 gpio-310 (P23_6               )
 gpio-311 (P23_7               )
 gpio-312 (P24_0               )
 gpio-313 (P24_1               )
 gpio-314 (P24_2               )
 gpio-315 (P24_3               )
 gpio-316 (P24_4               )
 gpio-317 (P24_5               )
 gpio-318 (P24_6               )
 gpio-319 (P24_7               )
 gpio-320 (P25_0               )
 gpio-321 (P25_1               )
 gpio-322 (P25_2               )
 gpio-323 (P25_3               )
 gpio-324 (P25_4               )
 gpio-325 (P25_5               )
 gpio-326 (P25_6               )
 gpio-327 (P25_7               )
 gpio-328 (P26_0               )
 gpio-329 (P26_1               )
 gpio-330 (P26_2               )
 gpio-331 (P26_3               )
 gpio-332 (P26_4               )
 gpio-333 (P26_5               )
 gpio-334 (P26_6               )
 gpio-335 (P26_7               )
 gpio-336 (P27_0               )
 gpio-337 (P27_1               )
 gpio-338 (P27_2               )
 gpio-339 (P27_3               )
 gpio-340 (P27_4               )
 gpio-341 (P27_5               )
 gpio-342 (P27_6               )
 gpio-343 (P27_7               )
 gpio-344 (P28_0               )
 gpio-345 (P28_1               )
 gpio-346 (P28_2               )
 gpio-347 (P28_3               )
 gpio-348 (P28_4               )
 gpio-349 (P28_5               )
 gpio-350 (P28_6               )
 gpio-351 (P28_7               )
 gpio-352 (P29_0               )
 gpio-353 (P29_1               )
 gpio-354 (P29_2               )
 gpio-355 (P29_3               )
 gpio-356 (P29_4               )
 gpio-357 (P29_5               )
 gpio-358 (P29_6               )
 gpio-359 (P29_7               )
 gpio-360 (P30_0               )
 gpio-361 (P30_1               )
 gpio-362 (P30_2               )
 gpio-363 (P30_3               )
 gpio-364 (P30_4               )
 gpio-365 (P30_5               )
 gpio-366 (P30_6               )
 gpio-367 (P30_7               )
 gpio-368 (P31_0               )
 gpio-369 (P31_1               )
 gpio-370 (P31_2               )
 gpio-371 (P31_3               )
 gpio-372 (P31_4               )
 gpio-373 (P31_5               )
 gpio-374 (P31_6               )
 gpio-375 (P31_7               )
 gpio-376 (P32_0               )
 gpio-377 (P32_1               )
 gpio-378 (P32_2               )
 gpio-379 (P32_3               )
 gpio-380 (P32_4               )
 gpio-381 (P32_5               )
 gpio-382 (P32_6               )
 gpio-383 (P32_7               )
 gpio-384 (P33_0               )
 gpio-385 (P33_1               )
 gpio-386 (P33_2               )
 gpio-387 (P33_3               )
 gpio-388 (P33_4               )
 gpio-389 (P33_5               )
 gpio-390 (P33_6               )
 gpio-391 (P33_7               )
 gpio-392 (P34_0               )
 gpio-393 (P34_1               )
 gpio-394 (P34_2               )
 gpio-395 (P34_3               )
 gpio-396 (P34_4               )
 gpio-397 (P34_5               )
 gpio-398 (P34_6               )
 gpio-399 (P34_7               )
 gpio-400 (P35_0               )
 gpio-401 (P35_1               )
 gpio-402 (P35_2               )
 gpio-403 (P35_3               )
 gpio-404 (P35_4               )
 gpio-405 (P35_5               )
 gpio-406 (P35_6               )
 gpio-407 (P35_7               )
 gpio-408 (P36_0               )
 gpio-409 (P36_1               )
 gpio-410 (P36_2               )
 gpio-411 (P36_3               )
 gpio-412 (P36_4               )
 gpio-413 (P36_5               )
 gpio-414 (P36_6               )
 gpio-415 (P36_7               )
 gpio-416 (P37_0               )
 gpio-417 (P37_1               )
 gpio-418 (P37_2               )
 gpio-419 (P37_3               )
 gpio-420 (P37_4               )
 gpio-421 (P37_5               )
 gpio-422 (P37_6               )
 gpio-423 (P37_7               )
 gpio-424 (P38_0               )
 gpio-425 (P38_1               )
 gpio-426 (P38_2               )
 gpio-427 (P38_3               )
 gpio-428 (P38_4               )
 gpio-429 (P38_5               )
 gpio-430 (P38_6               )
 gpio-431 (P38_7               )
 gpio-432 (P39_0               )
 gpio-433 (P39_1               )
 gpio-434 (P39_2               )
 gpio-435 (P39_3               )
 gpio-436 (P39_4               )
 gpio-437 (P39_5               )
 gpio-438 (P39_6               )
 gpio-439 (P39_7               )
 gpio-440 (P40_0               )
 gpio-441 (P40_1               )
 gpio-442 (P40_2               )
 gpio-443 (P40_3               )
 gpio-444 (P40_4               )
 gpio-445 (P40_5               )
 gpio-446 (P40_6               )
 gpio-447 (P40_7               )
 gpio-448 (P41_0               )
 gpio-449 (P41_1               )
 gpio-450 (P41_2               )
 gpio-451 (P41_3               )
 gpio-452 (P41_4               )
 gpio-453 (P41_5               )
 gpio-454 (P41_6               )
 gpio-455 (P41_7               )
 gpio-456 (P42_0               )
 gpio-457 (P42_1               )
 gpio-458 (P42_2               )
 gpio-459 (P42_3               |phy-reset           ) out hi
 gpio-460 (P42_4               |phy-reset           ) out hi
 gpio-461 (P42_5               )
 gpio-462 (P42_6               )
 gpio-463 (P42_7               )
 gpio-464 (P43_0               )
 gpio-465 (P43_1               )
 gpio-466 (P43_2               )
 gpio-467 (P43_3               )
 gpio-468 (P43_4               )
 gpio-469 (P43_5               )
 gpio-470 (P43_6               )
 gpio-471 (P43_7               )
 gpio-472 (P44_0               )
 gpio-473 (P44_1               )
 gpio-474 (P44_2               )
 gpio-475 (P44_3               )
 gpio-476 (P44_4               )
 gpio-477 (P44_5               )
 gpio-478 (P44_6               )
 gpio-479 (P44_7               )
 gpio-480 (P45_0               )
 gpio-481 (P45_1               )
 gpio-482 (P45_2               )
 gpio-483 (P45_3               )
 gpio-484 (P45_4               )
 gpio-485 (P45_5               )
 gpio-486 (P45_6               )
 gpio-487 (P45_7               )
 gpio-488 (P46_0               )
 gpio-489 (P46_1               )
 gpio-490 (P46_2               |reset               ) out hi ACTIVE LOW
 gpio-491 (P46_3               )
 gpio-492 (P46_4               )
 gpio-493 (P46_5               )
 gpio-494 (P46_6               )
 gpio-495 (P46_7               )
 gpio-496 (P47_0               )
 gpio-497 (P47_1               )
 gpio-498 (P47_2               )
 gpio-499 (P47_3               )
 gpio-500 (P47_4               )
 gpio-501 (P47_5               )
 gpio-502 (P47_6               )
 gpio-503 (P47_7               )
 gpio-504 (P48_0               )
 gpio-505 (P48_1               )
 gpio-506 (P48_2               )
 gpio-507 (P48_3               )
 gpio-508 (P48_4               )
 gpio-509 (P48_5               )
 gpio-510 (P48_6               )
 gpio-511 (P48_7               )
root@localhost:/sys/class/gpio#
```
