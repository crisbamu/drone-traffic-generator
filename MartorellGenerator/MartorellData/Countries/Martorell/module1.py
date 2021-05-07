with open("contour.xml") as f:
    f2 = open ("contour-v2.xml", 'w')
    for line in f.readlines():
##        if line.startswith("<coordinates>"):
##            new_line = line.replace("coordinates", "contour")
##            f2.write(new_line)
##            f2.write("<coordinates>\n")
##        elif line[0].isdigit():
        parts=line.strip().split(',')
        if len(parts)==2:
            f2.write("\t\t<point>"+parts[1]+","+parts[0]+"</point>\n")
##        elif line.startswith("<coordinates>"):
##            f2.write("</contour>\n")
        else:
            f2.write(line)
    
    f2.close()

with open("airspace.xml") as f:
    f2 = open ("airspace-v2.xml", 'w')
    for line in f.readlines():
        if line.startswith("<coordinates>"):
            points = line.replace("<coordinates>", "").strip("</coordinates>\n").split()
            for point in points:
                lon, lat = point.split(',')
                f2.write("<point>"+lat+","+lon+"</point>\n")
        elif line.startswith("<prohibited-areas>"):
            f2.write(line)
            f2.write("<polygons>\n")
        elif line.startswith("</prohibited-areas>"):
            f2.write("</polygons>\n")
            f2.write(line)
        else:
            f2.write(line)
    
    f2.close()
