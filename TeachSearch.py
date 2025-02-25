# -*- coding: utf-8 -*-

import networkx as nx
import matplotlib.pyplot as plt

def BFS(testing, G, nodes, neighbours, positions, Qpositions, color_map):
    print("\nDemonstrating Breadth-First Search\n")
    
    # initialise graph to display queue
    Q=nx.Graph()
    # queue stores queued nodes and their states
    queue = []
    # visited stores nodes once they've been visited
    visited = []

    # node_val is the start node on the graph
    node_val = int(input("Please enter start node: "))
    # nodeIndex is the index of the nodeVal node in the nodes array
    node_index = 0
    
    # determine the index of nodeVal node in graph
    # generalised to accommodate node values that differ from index values
    count = 0
    for x in nodes:
        if x[0] == node_val:
            node_index = count
        count += 1
        
    # default to first node if nodeVal > largest node
    if node_val > len(nodes):
        node_val = nodes[0][0]
    
    # display initial graph and queue
    nx.draw_networkx(G, positions, node_color=color_map,with_labels=1)
    nx.draw_networkx(Q, Qpositions)
    plt.pause(1)

    # add the start node to the queue data structure and update its state
    queue.append([node_index, nodes[node_index]])
    nodes[node_index][1] = "queued"
    # update the display queue
    Q.add_node(nodes[node_index][0])
    count = 0
    # reshuffle queue positions to align with edge of window
    for x in queue:
        Qpositions.update({x[1][0]: [0.2*count, 2.5]})
        count += 1
    # update colour map to correspond to change of state
    color_map[node_val] = "yellow"
    # show updated display
    nx.draw_networkx(G, positions, node_color=color_map,with_labels=1)
    nx.draw_networkx(Q, Qpositions)
    plt.pause(1)

    # perform BF search
    while queue != []:
        # for the each node in the queue, add any unvisited neighbours to the queue
        for current_neighbour in neighbours[queue[0][0]]:
            node_index = 0 
            for current_node in nodes:
                if current_node[0] == current_neighbour and current_node[1] == "unvisited":
                    # in testing mode, get user to input node to visit
                    if testing == True:
                        correct = False
                        answer_node = int(input("Input which node should be queued next: "))
                        while(correct == False):
                            if(answer_node == current_node[0]):
                                print("Correct!")
                                correct = True
                            else:
                                answer_node = int(input("Incorrect! Try again: "))
                    # add node to back of queue and update state
                    current_node[1] = "queued"
                    queue.append([node_index, current_node])
                    Q.add_node(current_node[0])
                    count = 0
                    # update positions of display queue
                    for x in queue:
                        Qpositions.update({x[1][0]: [0.2*count, 2.5]})
                        count += 1
                    # show updated queue and graph
                    nx.draw_networkx(Q, Qpositions)
                    # colour node yellow to show queued status
                    color_map[current_node[0]] = "yellow"
                    nx.draw_networkx(G, positions, node_color=color_map,with_labels=1)
                    plt.pause(1)
                node_index += 1
            node_index = 0
        # once all neighbours are visited, mark current node visited and remove from queue
        if testing == True:
            correct = False
            answer_node = int(input("Input which node should be visited next: "))
            while(correct == False):
                if(answer_node == nodes[queue[0][0]][0]):
                    print("Correct!")
                    correct = True
                else:
                    answer_node = int(input("Incorrect! Try again: "))
        # update status to visited
        nodes[queue[0][0]][1] = "visited"
        index = queue[0][0]
        visited.append(nodes[queue[0][0]])
        color_map[nodes[queue[0][0]][0]] = "lightgreen"
        queue.pop(0)
        Q.remove_node(nodes[index][0])
        count = 0
        # display updated queue and graph
        for x in queue:
            Qpositions.update({x[1][0]: [0.2*count, 2.5]})
            count += 1
        nx.draw_networkx(G, positions, node_color=color_map,with_labels=1)
        nx.draw_networkx(Q, Qpositions)
        plt.pause(1)
        
    plt.pause(1)
    
    
def DFS_recur(G, nodes, neighbours, positions, stack, S, Spositions, index, color_map, testing):
    # for each node, check its first neighbour, and then that neighbour's neighbour, etc.
    # recursion ends when we reach a node with no other neighbours
    for current_neighbour in neighbours[index]:
        node_index = 0
        for current_node in nodes:
            # add neighbour to the stack
            if current_node[0] == current_neighbour and current_node[1] == "unvisited":
                if testing == True:
                    correct = False
                    answer_node = int(input("Input which node should be added to the stack: "))
                    while(correct == False):
                        if(answer_node == current_node[0]):
                            print("Correct!")
                            correct = True
                        else:
                            answer_node = int(input("Incorrect! Try again: "))
                # update status and display stack to show stacked node
                current_node[1] = "stacked"
                stack.append([node_index, current_node])
                color_map[current_node[0]] = "yellow"
                S.add_node(current_node[0])
                count = 0
                for x in stack:
                    Spositions.update({x[1][0]: [2.5, 0.2*count]})
                    count += 1
                nx.draw_networkx(G, positions, node_color=color_map,with_labels=1)
                nx.draw_networkx(S, Spositions)
                plt.pause(1)
                # function calls itself until no further neighbours found
                DFS_recur(G, nodes, neighbours, positions, stack, S, Spositions, node_index, color_map, testing)
            node_index += 1
    
    # testing question asks used to identify fully visited node
    if testing == True:
        correct = False
        answerNode = int(input("Input which node should be marked visited: "))
        while(correct == False):
            if(answerNode == nodes[index][0]):
                print("Correct!")
                correct = True
            else:
                answerNode = int(input("Incorrect! Try again: "))
                
    # update node's status and displays accordingly
    color_map[nodes[index][0]] = "lightgreen"
    stack.pop()
    S.remove_node(nodes[index][0])
    count = 0
    for x in stack:
        Spositions.update({x[1][0]: [2.5, 0.2*count]})
        count += 1
    nx.draw_networkx(G, positions, node_color=color_map,with_labels=1)
    nx.draw_networkx(S, Spositions)
    plt.pause(1)
    
    
def DFS_init(testing, G, nodes, neighbours, positions, Spositions, color_map):
    
    print("\nDemonstrating Depth-First Search\n")
    
    S=nx.Graph()    
    stack = []
    index = 0
    
    nodeVal = input("Please enter start node: ")
    nodeVal = int(nodeVal)
    count = 0
        
    # sanitise data
    if nodeVal > len(nodes):
        nodeVal = 0
        
    for x in nodes:
        if x[0] == nodeVal:
            index = count
        count += 1
    
    # show graph with start node stacked
    nx.draw_networkx(G, positions, node_color=color_map,with_labels=1)
    plt.pause(1)
    
    nodes[index][1] = "stacked"
    stack.append([index, nodes[index]])
    S.add_node(nodes[index][0])

    color_map[nodeVal] = "yellow"
    nx.draw_networkx(G, positions, node_color=color_map,with_labels=1)
    nx.draw_networkx(S, Spositions)
    plt.pause(1)
    
    # call recursive DFS function
    DFS_recur(G, nodes, neighbours, positions, stack, S, Spositions, index, color_map, testing)
    
    print("\n")


# initialise colour maps, used by NetworkX to determine the colour of each node
color_map = ['lightblue', 'lightblue', 'lightblue', 'lightblue', 'lightblue']
color_map_2 = ['lightblue', 'lightblue', 'lightblue', 'lightblue', 'lightblue', 'lightblue', 'lightblue']

# initialise node values and states
nodes = [[1, "unvisited"],[3, "unvisited"],[2, "unvisited"],[4, "unvisited"],[0, "unvisited"]]
nodes2 = [[0,"unvisited"],[1,"unvisited"],[2,"unvisited"],[3,"unvisited"],[4,"unvisited"],[5,"unvisited"],[6,"unvisited"]]

# list of neighbours used by search algorithms to update stack or queue
neighbours = [[0,2,3],[1,4],[0,1,4],[2,3],[1,2]]
neighbours2 = [[1,2],[0,3,4],[0,5,6],[1],[1],[2],[2]]

# positions denote where the nodes should be positioned on the screen
positions = {0: [0,1], 1: [1,0], 2: [1,2], 3: [2,0], 4: [2,2]}
Qpositions = {0: [0,2.5], 1: [0.2,2.5], 2: [0.4,2.5], 3: [0.6,2.5], 4: [0.8,2.5]}
Spositions = {0: [2.5,0], 1: [2.5,0.2], 2: [2.5,0.4], 3: [2.5,0.6], 4: [2.5,0.8]}

positions2 = {0:[0.9,2], 1:[0.3,1], 2:[1.5,1], 3:[0,0], 4:[0.6,0], 5:[1.2,0], 6:[1.8,0]}
Qpositions2 = {0: [0,2.5], 1: [0.3,2.5], 2: [0.6,2.5], 3: [0.9,2.5], 4: [1.2,2.5], 5: [1.5,2.5], 6: [1.8,2.5]}
Spositions2 = {0: [2.5,0], 1: [2.5,0.3], 2: [2.5,0.6], 3: [2.5,0.9], 4: [2.5,1.2], 5: [2.5,1.5], 6: [2.5,1.8]}

# initialise graph 1
G=nx.Graph()
for i in range(0,4):
    G.add_node(i)
G.add_edge(0,1)
G.add_edge(1,2)
G.add_edge(0,2)
G.add_edge(1,3)
G.add_edge(2,4)
G.add_edge(3,4)

# initialise graph 2
G2=nx.Graph()
for i in range(0,6):
    G2.add_node(i)
G2.add_edge(0,1)
G2.add_edge(0,2)
G2.add_edge(1,3)
G2.add_edge(1,4)
G2.add_edge(2,5)
G2.add_edge(2,6)

# initialise menu condition
exit_condit = False

print("Welcome to TeachSearch")
input("\nPress enter to view Graph 1")
nx.draw_networkx(G, positions, node_color=color_map, with_labels=1)
plt.pause(1)
input("\nPress enter to view Graph 2")
nx.draw_networkx(G2, positions2, node_color=color_map_2, with_labels=1)
plt.pause(1)

# loop through menu until user selects to exit
while(exit_condit==False):
    print("\nLEARNING MODE")
    print("1: BFS Demo")
    print("2: DFS Demo")
    print("TESTING MODE")
    print("3: BFS Test")
    print("4: DFS Test")
    print("5: Exit program\n")
    menu_choice = input("Please select an option (1-5): ")
    if menu_choice != "5":
        graph_choice = input("Please select a graph to traverse (1 or 2): ")

    match menu_choice:
        case "1":
            if graph_choice == "1":
                BFS(False, G, nodes, neighbours, positions, Qpositions, color_map)
            elif graph_choice == "2":
                BFS(False, G2, nodes2, neighbours2, positions2, Qpositions2, color_map_2)
        case "2":
            if graph_choice == "1":
                DFS_init(False, G, nodes, neighbours, positions, Spositions, color_map)
            elif graph_choice == "2":
                DFS_init(False, G2, nodes2, neighbours2, positions2, Spositions2, color_map_2)
        case "3":
            if graph_choice == "1":
                BFS(True, G, nodes, neighbours, positions, Qpositions, color_map)
            elif graph_choice == "2":
                BFS(True, G2, nodes2, neighbours2, positions2, Qpositions2, color_map_2)
        case "4":
            if graph_choice == "1":
                DFS_init(True, G, nodes, neighbours, positions, Spositions, color_map)
            elif graph_choice == "2":
                DFS_init(True, G2, nodes2, neighbours2, positions2, Spositions2, color_map_2)
        case "5":
            exit_condit = True
        case _:
            print("Error! Please enter a valid option")
            
    # reset colour maps for next loop
    color_map = ['lightblue', 'lightblue', 'lightblue', 'lightblue', 'lightblue']
    color_map_2 = ['lightblue', 'lightblue', 'lightblue', 'lightblue', 'lightblue', 'lightblue', 'lightblue']

    # reset node values and states
    nodes = [[1, "unvisited"],[3, "unvisited"],[2, "unvisited"],[4, "unvisited"],[0, "unvisited"]]
    nodes2 = [[0,"unvisited"],[1,"unvisited"],[2,"unvisited"],[3,"unvisited"],[4,"unvisited"],[5,"unvisited"],[6,"unvisited"]]
