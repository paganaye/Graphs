# Canonical Graph Signature Algorithm with Node Grouping

## Summary of Steps

1. **Initialize Base Signatures**:
    - For each node, calculate its **degree** (number of edges).
    - Store these initial signatures as a sorted descending list of degrees.

2. **Identify Duplicate Signatures**:
    - Sort the list and identify groups of nodes with identical degrees (e.g., all nodes with degree 2).

3. **Expand Duplicate Signatures by Group**:
    - For each group of nodes with identical signatures:
        - **Within the group**, start expanding each node’s signature by examining its neighbors.
        - For each neighbor of a node:
            - Include the neighbor's degree.
              - Mark already-visited nodes **within this group**, using `-1` for the first node seen, `-2` for the second, and so on.
                **These markers are local to each group** and ensure that the processing remains isolated and consistent within each duplicate group.
        - if no group can be expanded then the algorithm is complete
        - otherwise sort each node's expanded signature within the group, ensuring a consistent order.
       
4. **Update and Sort the Global Signature**:
    - Replace the base signatures with the newly expanded signatures.
    - Sort the global list of expanded signatures.

5. **Check Uniqueness**:
    - If all signatures are now unique, stop.
    - If any duplicates remain, repeat the expansion step **within their respective groups**.

6. **Return the Canonical Signature**:
    - The final sorted list of unique signatures represents the graph’s canonical signature.

---

## Example Using the Graph `A-B, A-C, B-C, A-D, D-E`

1. **Initial Base Signatures**:
    - Calculate the degree of each node: `A (3), B (2), C (2), D (2), E (1)`.
    - Initial signature list: `[3, 2, 2, 2, 1]`.

2. **Identify Duplicate Groups**:
    - Nodes B, C, and D each have a degree of 2, forming a group.

3. **Expand Signatures by Group**:
    - **Group for Node B**:
        - Initial signature: `[3, 2]` based on neighbors A and C.
        - Expansion: `[[ -1, 2, 2], [ -1, 3 ]]` (with `-1` marking visited nodes locally within this group).
    - **Group for Node C**:
        - Initial signature: `[3, 2]` based on neighbors A and B.
        - Expansion: `[[ -1, 2, 2], [ -2, 2, 3 ]]` (using group-local `-1` and `-2` markers for consistency within this group).
    - **Group for Node D**:
        - Initial signature: `[3, 1]` based on neighbors A and E.

    - The global signature list after expansion is `[3, [[ -1, 2, 2], [ -2, 2, 3 ]], [[ -1, 2, 2], [ -1, 3 ]], [3, 1], 1]`.

4. **Final Canonical Signature**:
    - After sorting and ensuring all signatures are unique, the final canonical signature is `[3, [[ -1, 2, 2], [ -2, 2, 3 ]], [[ -1, 2, 2], [ -1, 3 ]], [3, 1], 1]`. This sorted, unique list represents the graph’s **canonical signature**.

By handling expansions and marks **within each group**, we ensure consistent, isolated processing, yielding a unique, canonical signature for the graph.

